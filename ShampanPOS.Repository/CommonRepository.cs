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

//        public async Task<ResultVM> AreaLocationList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

//                // Base query
//                string query = @"
//            SELECT DISTINCT 
//                ISNULL(L.Id, 0) AS Id, 
//                ISNULL(L.Code, '') AS Code, 
//                ISNULL(L.Name, '') AS Name, 
//                ISNULL(L.ParentId, 0) AS ParentId, 
//                ISNULL(L.EnumType, '') AS EnumType
//";

//                if (conditionalValues != null && conditionalValues.Length > 0 && conditionalValues[0] != null)
//                {
//                    if (conditionalValues[0] == "Division")
//                    {
//                        query += @"
//                 ,ISNULL(Country.Name, '') AS CountryName";
//                    }
//                    if (conditionalValues[0] == "District")
//                    {
//                        query += @"
//                 ,ISNULL(Country.Name, '') AS CountryName, 
//                  ISNULL(Division.Name, '') AS DivisionName";
//                    }
//                    if (conditionalValues[0] == "Thana")
//                    {
//                        query += @"
//                 ,ISNULL(Country.Name, '') AS CountryName,  
//                  ISNULL(Division.Name, '') AS DivisionName, 
//                    ISNULL(District.Name, '') AS DistrictName ";
//                    }
//                }

//                query += @"
//        FROM Locations L
//        LEFT OUTER JOIN EnumTypes E ON L.EnumType = E.EnumType ";

//                if (conditionalValues != null && conditionalValues.Length > 0 && conditionalValues[0] != null)
//                {
//                    if (conditionalValues[0] == "Division")
//                    {
//                        query += @"
//                        LEFT OUTER JOIN Locations Country ON L.ParentId = Country.Id ";
//                    }
//                    if (conditionalValues[0] == "District")
//                    {
//                        query += @"
//                        LEFT OUTER JOIN Locations Division ON L.ParentId = Division.Id 
//                        LEFT OUTER JOIN Locations Country ON Division.ParentId = Country.Id ";
//                    }
//                    if (conditionalValues[0] == "Thana")
//                    {
//                        query += @"
//                        LEFT OUTER JOIN Locations Division ON L.ParentId = Division.Id  
//                        LEFT OUTER JOIN Locations District ON Division.ParentId = District.Id 
//                        LEFT OUTER JOIN Locations Country ON District.ParentId = Country.Id ";
//                    }
//                }

//                query += @"
//        WHERE 1 = 1 ";

//                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

//                objComm.Fill(dataTable);

//                var modelList = dataTable.AsEnumerable().Select(row => new LocationVM
//                {
//                    Id = Convert.ToInt32(row["Id"]),
//                    Name = row["Name"]?.ToString(),
//                    ParentId = Convert.ToInt32(row["ParentId"]),
//                    EnumType = row["EnumType"]?.ToString(),
//                    CountryName = conditionalValues != null && conditionalValues.Length > 0 && conditionalValues[0] == "Division" || conditionalValues[0] == "District" || conditionalValues[0] == "Thana" ? row["CountryName"]?.ToString() : null,
//                    DivisionName = conditionalValues != null && conditionalValues.Length > 0 && conditionalValues[0] == "District" || conditionalValues[0] == "Thana" ? row["DivisionName"]?.ToString() : null,
//                    DistrictName = conditionalValues != null && conditionalValues.Length > 0 && conditionalValues[0] == "Thana" ? row["DistrictName"]?.ToString() : null
//                }).ToList();

//                // Return success result
//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = modelList;
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

        //public async Task<ResultVM> ParentSalePersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string query = @"

        //              SELECT 
        //        '' AS Code, 
        //        0 AS Id, 
        //        'All' AS Name, 
        //        '' AS Mobile,
        //        0 AS BranchId
                
    
        //    UNION ALL
        //    SELECT DISTINCT
        //        ISNULL(M.Code, '') AS Code,
        //        ISNULL(M.Id, 0) AS Id,
        //        ISNULL(M.Name, '') AS Name,
        //        ISNULL(M.Mobile, '') AS Mobile,
        //       ISNULL(M.BranchId, 0) BranchId
   
        //    FROM SalesPersons M
        //    WHERE M.IsActive = 1";


        //        query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //        SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new SalesPersonVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Name = row["Name"]?.ToString(),
        //            Code = row["Code"]?.ToString(),
        //            Mobile = row["Mobile"]?.ToString(),


        //        }).ToList();


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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

        //public async Task<ResultVM> GetFiscalYearForSaleList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //    SELECT DISTINCT
        //        ISNULL(H.Id,0)	Id,
        //        ISNULL(H.Year,'')	Year,
        //        ISNULL(CONVERT(VARCHAR(10), H.YearStart, 120), '') AS YearStart, 
        //        ISNULL(CONVERT(VARCHAR(10), H.YearEnd, 120), '') AS YearEnd,
        //        ISNULL(H.Remarks,'') Remarks
        //        FROM FiscalYearForSales H
        //          WHERE 1 =1";


        //        query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //        SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new FiscalYearForSaleVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Year = Convert.ToInt32(row["Year"]),
        //            YearStart = row["YearStart"]?.ToString(),
        //            YearEnd = row["YearEnd"]?.ToString(),


        //        }).ToList();


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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
        //public async Task<ResultVM> SalePersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string sqlQuery = @"
        //              SELECT DISTINCT
        //                 ISNULL(H.Id, 0) AS Id,
        //                 ISNULL(H.Name, '') AS Name,
        //                 ISNULL(H.Code, '') AS Code,
        //                 ISNULL(H.BranchId, 1) AS BranchId,
        //              ISNULL(H.BanglaName, '') AS BanglaName,
        //              ISNULL(H.BanglaName, '') AS BanglaName,
        //              CASE WHEN ISNULL(H.IsActive,0) = 1 THEN 'Active' ELSE 'Inactive'	END Status
        //             FROM SalesPersons H
        //             WHERE H.IsArchive != 1
        //        ";


        //        sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

        //        SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new SalesPersonVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Name = row["Name"]?.ToString(),
        //            Code = row["Code"]?.ToString(),
        //            BanglaName = row["BanglaName"]?.ToString(),
        //            BranchId = Convert.ToInt32(row["BranchId"]?.ToString())

        //        }).ToList();


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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




        //  public async Task<ResultVM> SaleOrderList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //  {
        //      bool isNewConnection = false;
        //      DataTable dataTable = new DataTable();
        //      ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //      try
        //      {
        //          if (conn == null)
        //          {
        //              conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //              conn.Open();
        //              isNewConnection = true;
        //          }

        //          string sqlQuery = @"
        //                SELECT DISTINCT
        //                   ISNULL(H.Id, 0) AS Id,
        //                   ISNULL(H.Code, '') AS Code,
        //                   ISNULL(H.BranchId, 1) AS BranchId,
        //                ISNULL(H.CustomerId, '') AS CustomerId,
        // ISNULL(C.Name, '')AS CustomerName
        //               FROM Sales H
        //LEFT OUTER JOIN Customers C ON H.CustomerId= C.Id
        //          ";


        //          sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

        //          SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

        //          // SET additional conditions param
        //          objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //          objComm.Fill(dataTable);

        //          var modelList = dataTable.AsEnumerable().Select(row => new SaleVM
        //          {
        //              Id = Convert.ToInt32(row["Id"]),
        //              Name = row["CustomerName"]?.ToString(),
        //              Code = row["Code"]?.ToString(),
        //              BranchId = Convert.ToInt32(row["BranchId"]?.ToString())

        //          }).ToList();


        //          result.Status = "Success";
        //          result.Message = "Data retrieved successfully.";
        //          result.DataVM = modelList;
        //          return result;
        //      }
        //      catch (Exception ex)
        //      {
        //          result.ExMessage = ex.Message;
        //          result.Message = ex.Message;
        //          return result;
        //      }
        //      finally
        //      {
        //          if (isNewConnection && conn != null)
        //          {
        //              conn.Close();
        //          }
        //      }
        //  }









        //public async Task<ResultVM> GetSalePersonParentList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        // Base query for SalesPersons
        //        string sqlQuery = @"
        //            SELECT DISTINCT
        //                ISNULL(H.Id, 0) AS Id,
        //                ISNULL(H.Name, '') AS Name,
        //                ISNULL(H.Code, '') AS Code,
        //                ISNULL(H.BranchId, 1) AS BranchId,
        //                ISNULL(H.BanglaName, '') AS BanglaName,
        //                CASE WHEN ISNULL(H.IsActive,0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status
        //            FROM SalesPersons H
        //            WHERE H.IsArchive != 1
        //            ";

        //        // Apply dynamic conditions
        //        sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

        //        // Add UNION ALL for Id = 0 record
        //        sqlQuery += @"
        //            UNION ALL
        //            SELECT
        //                H.Id,
        //                H.Name,
        //                H.Code,
        //                @BranchId AS BranchId,
        //                H.BanglaName,
        //                CASE WHEN ISNULL(H.IsActive,0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status
        //            FROM SalesPersons H
        //            WHERE H.Id = 0
        //            ";

        //        // Add ORDER BY H.Id ascending at the end
        //        sqlQuery += @"
        //            ORDER BY Id ASC
        //            ";

        //        SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

        //        // Set additional parameters for dynamic conditions
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

        //        // Add BranchId parameter for the UNION ALL record
        //        objComm.SelectCommand.Parameters.AddWithValue("@BranchId", conditionalValues.Contains("BranchId")
        //            ? Convert.ToInt32(conditionalValues[Array.IndexOf(conditionalFields, "BranchId")])
        //            : 1);

        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new SalesPersonVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Name = row["Name"]?.ToString(),
        //            Code = row["Code"]?.ToString(),
        //            BanglaName = row["BanglaName"]?.ToString(),
        //            BranchId = Convert.ToInt32(row["BranchId"]?.ToString())
        //        }).ToList();

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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


        //public async Task<ResultVM> CurrencieList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string sqlQuery = @"
        //              SELECT DISTINCT
        //                 ISNULL(H.Id, 0) AS Id,
        //                 ISNULL(H.Name, '') AS Name,
        //                 ISNULL(H.Code, '') AS Code,
	       //              CASE WHEN ISNULL(H.IsActive,0) = 1 THEN 'Active' ELSE 'Inactive'	END Status
        //             FROM Currencies H
        //             WHERE H.IsArchive != 1
        //        ";


        //        sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

        //        SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new CurrencyVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Name = row["Name"]?.ToString(),
        //            Code = row["Code"]?.ToString()

        //        }).ToList();


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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

        public async Task<ResultVM> CustomerCategoryList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                         ISNULL(H.Name, '') AS Name,
                         ISNULL(H.EffectDate, '') AS EffectDate

                     FROM ProductPriceGroups H
                     WHERE 1 = 1


                ";


                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new CustomerCategoryVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString()
                    //Code = row["Code"]?.ToString()

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


        //public async Task<ResultVM> DeliveryPersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string sqlQuery = @"
        //                      SELECT DISTINCT
        //                      ISNULL(H.Id, 0) AS Id,
        //                      ISNULL(H.Id, 0) AS Value,
        //                      ISNULL(H.Code, '') AS Code,
        //                      ISNULL(H.Name, '') AS Name,
        //                      ISNULL(H.Comments, '') AS Comments

        //                      FROM DeliveryPersons H
        //        ";


        //        sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

        //        SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new CurrencyVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Name = row["Name"]?.ToString(),
        //            Code = row["Code"]?.ToString()

        //        }).ToList();


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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
        public async Task<ResultVM> ReceiveByDeliveryPersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

         ISNULL(H.Id,0)	Id
        ,ISNULL(H.Name,'') Name
        ,ISNULL(H.EnumType,'') EnumType

        FROM EnumTypes H

        WHERE H.EnumType = 'DeliveryPerson' 
                     ";


                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new CustomerAdvanceVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    EnumType = row["EnumType"]?.ToString()

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

       // public async Task<ResultVM> CustomerRouteList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
       // {
       //     bool isNewConnection = false;
       //     DataTable dataTable = new DataTable();
       //     ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

       //     try
       //     {
       //         if (conn == null)
       //         {
       //             conn = new SqlConnection(DatabaseHelper.GetConnectionString());
       //             conn.Open();
       //             isNewConnection = true;
       //         }

       //         string query = @"
						 //SELECT DISTINCT

       //                  ISNULL(H.Id,0)	Id
       //                 ,ISNULL(H.BranchId,0) BranchId
       //                 ,ISNULL(H.Code,'') Code
       //                 ,ISNULL(H.Name,'') Name
       //                 ,ISNULL(H.Address,'')	Address
       //                 ,ISNULL(H.BanglaName,'') BanglaName
		
       //                 FROM Routes H

       //                 WHERE IsActive = 1 AND IsArchive != 1";


       //         query = ApplyConditions(query, conditionalFields, conditionalValues, false);

       //         SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

       //         // SET additional conditions param
       //         objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



       //         objComm.Fill(dataTable);

       //         var modelList = dataTable.AsEnumerable().Select(row => new RouteVM
       //         {
       //             Id = Convert.ToInt32(row["Id"]),
       //             Name = row["Name"]?.ToString(),
       //             Code = row["Code"]?.ToString(),

       //         }).ToList();


       //         result.Status = "Success";
       //         result.Message = "Data retrieved successfully.";
       //         result.DataVM = modelList;
       //         return result;
       //     }
       //     catch (Exception ex)
       //     {
       //         result.ExMessage = ex.Message;
       //         result.Message = ex.Message;
       //         return result;
       //     }
       //     finally
       //     {
       //         if (isNewConnection && conn != null)
       //         {
       //             conn.Close();
       //         }
       //     }
       // }
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


        //public async Task<ResultVM> CampaignTargetList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }
        //        string sqlQuery = @"
        //         Select
        //         ISNULL(H.Id,0)	Id,
        //         ISNULL(H.TotalTarget,'') Value

        //        FROM SalePersonCampaignTargets H
        //        WHERE 1 =1 ";
        //        sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);
        //        SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);
        //        objComm.Fill(dataTable);
        //        var modelList = dataTable.AsEnumerable().Select(row => new SalePersonCampaignTargetVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Value = row["Value"]?.ToString()
        //        }).ToList();
        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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




        //        public async Task<ResultVM> SalePersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

        //                string query = @"

        //";


        //                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                // SET additional conditions param
        //                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //                objComm.Fill(dataTable);

        //                var modelList = dataTable.AsEnumerable().Select(row => new SalePersonRouteVM
        //                {
        //                    Id = Convert.ToInt32(row["Id"]),
        //                    Name = row["Name"]?.ToString(),


        //                }).ToList();


        //                result.Status = "Success";
        //                result.Message = "Data retrieved successfully.";
        //                result.DataVM = modelList;
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

        // GetProductModalForSaleData Method
        // GetProductModalForSaleData Method (Cleaned Version - No @FromDate)
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
                SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
                checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
                object res = checkCommand.ExecuteScalar();
                string customerCategory = res != null ? res.ToString() : "0";

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
    ISNULL(PBH.CostPrice,0) AS CostPrice, 
    ISNULL(PBH.SalesPrice,0) AS SalesPrice, 
    ISNULL(PBH.PurchasePrice,0) AS PurchasePrice, 
    ISNULL(PBH.SD,0) AS SD, 
    ISNULL(PBH.VATRate,0) AS VATRate, 
    ISNULL(stock.Quantity,0) AS QuantityInHand,
	ISNULL(CampaignRate.DiscountRate,0)DiscountRate,
    P.CtnSize AS UOMName,
    P.CtnSizeFactor AS UOMConversion,
    CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status

FROM Products P
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId
LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id

LEFT OUTER JOIN (
    SELECT BranchId, ProductId, SUM(Quantity) AS Quantity 
    FROM (
        SELECT 
            BranchId,
            ProductId,
            SUM(
                CASE 
                    WHEN Type = 'Purchase' THEN Quantity 
                    WHEN Type = 'Sales' THEN -Quantity 
                    ELSE 0 
                END
            ) AS Quantity
        FROM DayEndDetails
        WHERE BranchId = @BranchId
        GROUP BY BranchId, ProductId

        UNION ALL

        SELECT 
            ProductsOpeningStocks.BranchId,
            ProductId,
            SUM(OpeningQuantity) AS Quantity
        FROM ProductsOpeningStocks
        WHERE BranchId = @BranchId
		GROUP BY ProductsOpeningStocks.BranchId, ProductId

        UNION ALL

        SELECT 
            Purchases.BranchId,
            ProductId,
            SUM(Quantity) AS Quantity
        FROM PurchaseDetails
        LEFT OUTER JOIN Purchases ON Purchases.Id = PurchaseDetails.PurchaseId
        WHERE Purchases.BranchId = @BranchId
        GROUP BY Purchases.BranchId, ProductId

        UNION ALL

        SELECT 
            SaleOrders.BranchId,
            ProductId,
            SUM(Quantity) * -1 AS Quantity
        FROM SaleOrderDetails
        LEFT OUTER JOIN SaleOrders ON SaleOrders.Id = SaleOrderDetails.SaleOrderId
        WHERE SaleOrders.BranchId = @BranchId
        GROUP BY SaleOrders.BranchId, ProductId
    ) AS Result
    GROUP BY BranchId, ProductId
) stock ON P.Id = stock.ProductId

LEFT OUTER JOIN (
  SELECT ProductId ,DiscountRateBasedOnUnitPrice DiscountRate
FROM CampaignDetailByProductValues cd 
left outer join Campaigns c on c.Id=cd.CampaignId

WHERE @FromDate BETWEEN c.CampaignStartDate AND c.CampaignEndDate
        and c.BranchId =60
) CampaignRate ON P.Id = CampaignRate.ProductId


WHERE PBH.PriceCategory = @PriceCategory 
  AND PBH.BranchId = @BranchId 
  AND P.IsActive = 1
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
                if (customerCategory != null)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@PriceCategory", customerCategory);
                }

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
                    SDRate = row.Field<decimal>("SD"),
                    VATRate = row.Field<decimal>("VATRate"),
                    CostPrice = row.Field<decimal>("CostPrice"),
                    SalesPrice = row.Field<decimal>("SalesPrice"),
                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    UOMId = row.Field<int>("UOMId"),
                    QuantityInHand = row.Field<decimal>("QuantityInHand"),
                    UOMName = row.Field<string>("UOMName"),
                    UOMConversion = row.Field<string>("UOMConversion"),
                    Status = row.Field<string>("Status"),
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

        //        public async Task<ResultVM> GetProductModalForSaleData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

        //                //Add

        //                SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
        //                checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
        //                object res = checkCommand.ExecuteScalar();
        //                string customerCategory = res != null ? res.ToString() : "0";

        //                //End




        //                string query = @"
        //SELECT 
        //ISNULL(P.Id,0)ProductId , 
        //ISNULL(P.Name,'') ProductName,
        //ISNULL(P.BanglaName,'') BanglaName, 
        //ISNULL(P.Code,'') ProductCode, 
        //ISNULL(P.HSCodeNo,'') HSCodeNo,
        //ISNULL(P.ImagePath, '') AS ImagePath,
        //ISNULL(P.ProductGroupId,0) ProductGroupId,
        //ISNULL(PG.Name,'') ProductGroupName,
        //0 UOMId,
        //ISNULL(UOM.Name,'') UOMName,
        //ISNULL(PBH.CostPrice,0) CostPrice , 
        //ISNULL(PBH.SalesPrice,0) SalesPrice , 
        //ISNULL(PBH.PurchasePrice,0) PurchasePrice , 
        //ISNULL(PBH.SD,0) SD , 
        //ISNULL(PBH.VATRate,0) VATRate , 
        //ISNULL(stock.Quantity,0) QuantityInHand ,
        //P.CtnSize UOMName,
        //P.CtnSizeFactor UOMConversion,

        //CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END Status

        //FROM Products P
        //LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        //LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId
        //LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
        //left outer join (
        //Select BranchId,ProductId, SUM(Quantity)Quantity from  (SELECT 
        //    BranchId,
        //    ProductId,
        //    SUM(CASE 
        //            WHEN Type = 'Purchase' THEN Quantity 
        //            WHEN Type = 'Sales' THEN -Quantity 
        //            ELSE 0 
        //        END) AS Quantity
        //FROM 
        //    DayEndDetails
        //WHERE 
        //    BranchId = @BranchId
        //    AND Date <= @FromDate
        //GROUP BY 
        //    BranchId, 
        //    ProductId

        //UNION ALL


        //	SELECT 
        //    Purchases.BranchId,
        //    ProductId,
        //    SUM(Quantity)  AS Quantity
        //FROM 
        //    PurchaseDetails
        //LEFT OUTER JOIN 
        //    Purchases ON Purchases.Id = PurchaseDetails.PurchaseId
        //WHERE 
        //    Purchases.BranchId = @BranchId
        //    AND Purchases.PurchaseDate>= @FromDate
        //GROUP BY 
        //    Purchases.BranchId, 
        //    ProductId

        //	UNION ALL
        //SELECT 
        //    SaleOrders.BranchId,
        //    ProductId,
        //    SUM(Quantity)*-1  AS Quantity
        //FROM 
        //    SaleOrderDetails
        //LEFT OUTER JOIN 
        //    SaleOrders ON SaleOrders.Id = SaleOrderDetails.SaleOrderId
        //WHERE 
        //    SaleOrders.BranchId = @BranchId
        //    AND SaleOrders.InvoiceDateTime >= @FromDate
        //GROUP BY 
        //    SaleOrders.BranchId, 
        //    ProductId) as Result
        //	group by  BranchId,ProductId
        //) stock on p.id=stock.ProductId 

        //WHERE @param P.IsActive = 1 
        //";

        //                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
        //                {
        //                    query = query.Replace("@param", " PBH.PriceCategory = @PriceCategory AND PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
        //                }
        //                else
        //                {
        //                    query = query.Replace("@param", "");
        //                }

        //                // Apply additional conditions
        //                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
        //                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
        //                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                // SET additional conditions param
        //                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

        //                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
        //                }
        //                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
        //                }
        //                if (customerCategory != null)
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@PriceCategory", customerCategory);
        //                }

        //                objComm.Fill(dataTable);

        //                var modelList = dataTable.AsEnumerable().Select(row => new ProductDataVM
        //                {
        //                    ProductId = row.Field<int>("ProductId"),
        //                    ProductName = row.Field<string>("ProductName"),
        //                    BanglaName = row.Field<string>("BanglaName"),
        //                    ProductCode = row.Field<string>("ProductCode"),
        //                    HSCodeNo = row.Field<string>("HSCodeNo"),
        //                    ImagePath = row.Field<string>("ImagePath"),
        //                    ProductGroupId = row.Field<int>("ProductGroupId"),
        //                    SDRate = row.Field<decimal>("SD"),
        //                    VATRate = row.Field<decimal>("VATRate"),
        //                    CostPrice = row.Field<decimal>("CostPrice"),
        //                    SalesPrice = row.Field<decimal>("SalesPrice"),
        //                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
        //                    ProductGroupName = row.Field<string>("ProductGroupName"),
        //                    UOMId = row.Field<int>("UOMId"),
        //                    QuantityInHand = row.Field<decimal>("QuantityInHand"),
        //                    UOMName = row.Field<string>("UOMName"),
        //                    UOMConversion = row.Field<string>("UOMConversion"),
        //                    Status = row.Field<string>("Status")

        //                }).ToList();


        //                result.Status = "Success";
        //                result.Message = "Data retrieved successfully.";
        //                result.DataVM = modelList;
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

        // GetProductModalForSaleCountData Method
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
LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId
LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
WHERE PBH.PriceCategory = @PriceCategory 
  AND PBH.BranchId = @BranchId 
  AND P.IsActive = 1
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

        //        public async Task<ResultVM> GetProductModalForSaleCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

        //                //Add

        //                SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
        //                checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
        //                object res = checkCommand.ExecuteScalar();
        //                string customerCategory = res != null ? res.ToString() : "0";

        //                //End


        //                string query = @"
        //SELECT

        //COALESCE(COUNT(P.Id), 0) AS FilteredCount

        //FROM Products P
        //LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        //LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId
        //LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id

        //WHERE @param P.IsActive = 1 
        //";

        //                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
        //                {
        //                    query = query.Replace("@param", " PBH.PriceCategory = @PriceCategory AND PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
        //                }
        //                else
        //                {
        //                    query = query.Replace("@param", "");
        //                }

        //                // Apply additional conditions
        //                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                // SET additional conditions param
        //                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

        //                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
        //                }
        //                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
        //                }
        //                if (customerCategory != null)
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@PriceCategory", customerCategory);
        //                }

        //                objComm.Fill(dataTable);

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


        // GetProductModalForPurchaseData Method
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
ISNULL(PBH.CostPrice,0) CostPrice , 
ISNULL(PBH.SalesPrice,0) SalesPrice , 
ISNULL(PBH.PurchasePrice,0) PurchasePrice , 
ISNULL(PBH.SD,0) SD , 
ISNULL(PBH.VATRate,0) VATRate , 
ISNULL(P.CtnSize,'') UOMName,
ISNULL(P.CtnSizeFactor,0) UOMConversion,
CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END Status

FROM Products P
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN ProductPurchasePriceBatchHistories PBH ON P.Id = PBH.ProductId
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id

WHERE @param P.IsActive = 1 
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
                    SDRate = row.Field<decimal>("SD"),
                    VATRate = row.Field<decimal>("VATRate"),
                    CostPrice = row.Field<decimal>("CostPrice"),
                    SalesPrice = row.Field<decimal>("SalesPrice"),
                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    UOMId = row.Field<int>("UOMId"),
                    UOMName = row.Field<string>("UOMName"),
                    UOMConversion = row.Field<string>("UOMConversion"),
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

        // GetProductModalForPurchaseCountData Method
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
LEFT OUTER JOIN ProductPurchasePriceBatchHistories PBH ON P.Id = PBH.ProductId
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id

WHERE @param P.IsActive = 1 
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


        // GetProductModalData Method
        public async Task<ResultVM> GetProductModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    ISNULL(P.Id, 0) AS ProductId, 
                    ISNULL(P.Name, '') AS ProductName,
                    ISNULL(P.BanglaName, '') AS BanglaName, 
                    ISNULL(P.Code, '') AS ProductCode, 
                    ISNULL(P.HSCodeNo, '') AS HSCodeNo,
                    ISNULL(P.ProductGroupId, 0) AS ProductGroupId,
                    ISNULL(PG.Name, '') AS ProductGroupName,
                    ISNULL(P.UOMId, 0) AS UOMId,
                    ISNULL(UOM.Name, '') AS UOMName,
                    0.00 AS CostPrice, 
                    0.00 AS SalesPrice, 
                    0.00 AS PurchasePrice, 
                    0.00 AS SD, 
                    0.00 AS VATRate,

                    CASE 
                        WHEN CHARINDEX('×', P.CtnSize) > 0 
                             THEN LTRIM(SUBSTRING(P.CtnSize, CHARINDEX('×', P.CtnSize) + 1, LEN(P.CtnSize)))
                        ELSE P.CtnSize
                    END AS CtnSize,

                    CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status

                FROM Products P
                LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
                LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
                WHERE P.IsActive = 1";

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new ProductDataVM
                {
                    ProductId = row.Field<int>("ProductId"),
                    ProductName = row.Field<string>("ProductName"),
                    BanglaName = row.Field<string>("BanglaName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    HSCodeNo = row.Field<string>("HSCodeNo"),
                    ProductGroupId = row.Field<int>("ProductGroupId"),
                    SDRate = row.Field<decimal>("SD"),
                    VATRate = row.Field<decimal>("VATRate"),
                    CostPrice = row.Field<decimal>("CostPrice"),
                    SalesPrice = row.Field<decimal>("SalesPrice"),
                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    UOMId = row.Field<int>("UOMId"),
                    UOMName = row.Field<string>("UOMName"),
                    CtnSize = row.Field<string>("CtnSize"),
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

        // GetProductModalCountData Method
        public async Task<ResultVM> GetProductModalCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

WHERE P.IsActive = 1 ";


                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

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

        public async Task<ResultVM> GetTop10Customers(string branchId, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
                    SELECT TOP 10
                    S.CustomerId,
                    C.Name AS CustomerName,
                    FORMAT(SUM(S.GrandTotalAmount), 'N2') AS TotalGrandTotalAmount,
                    SUM(SD.Quantity) AS TotalQuantity
                    FROM Sales S
                    LEFT OUTER JOIN Customers C ON S.CustomerId = C.Id
                    LEFT OUTER JOIN SaleDetails SD ON S.Id = SD.SaleId
                    WHERE S.BranchId = @BranchId
                    AND S.CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- First day of the current month
                    AND S.CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) -- First day of next month
                    GROUP BY S.CustomerId, C.Name
                    ORDER BY SUM(S.GrandTotalAmount) DESC;
                    ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Assign transaction if exists
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }

                    cmd.Parameters.AddWithValue("@BranchId", branchId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                var modelList = dataTable.AsEnumerable().Select(row => new CustomerSalesModel
                {
                    CustomerId = row["CustomerId"] != DBNull.Value ? Convert.ToInt32(row["CustomerId"]) : 0,
                    CustomerName = row["CustomerName"]?.ToString(),
                    TotalGrandTotalAmount = row["TotalGrandTotalAmount"] != DBNull.Value
                        ? Convert.ToDecimal(row["TotalGrandTotalAmount"])
                        : 0,
                    TotalQuantity = row["TotalQuantity"] != DBNull.Value
                        ? Convert.ToInt32(row["TotalQuantity"])
                        : 0
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }

        public async Task<ResultVM> GetBottom10Customers(string? branchId, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
                        SELECT TOP 10
                        S.CustomerId,
                        C.Name AS CustomerName,
                        FORMAT(SUM(S.GrandTotalAmount), 'N2') AS TotalGrandTotalAmount,
                        SUM(SD.Quantity) AS TotalQuantity
                    FROM Sales S
                    LEFT OUTER JOIN Customers C ON S.CustomerId = C.Id
                    LEFT OUTER JOIN SaleDetails SD ON S.Id = SD.SaleId
                    WHERE S.BranchId = @BranchId
                    AND S.CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- First day of the current month
                    AND S.CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) -- First day of next month
                    GROUP BY S.CustomerId, C.Name
                    ORDER BY SUM(S.GrandTotalAmount) ASC;
                     ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Assign transaction if exists
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }

                    cmd.Parameters.AddWithValue("@BranchId", branchId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                var modelList = dataTable.AsEnumerable().Select(row => new CustomerSalesModel
                {
                    CustomerId = row["CustomerId"] != DBNull.Value ? Convert.ToInt32(row["CustomerId"]) : 0,

                    CustomerName = row["CustomerName"]?.ToString(),
                    TotalGrandTotalAmount = row["TotalGrandTotalAmount"] != DBNull.Value
                        ? Convert.ToDecimal(row["TotalGrandTotalAmount"])
                        : 0,
                    TotalQuantity = row["TotalQuantity"] != DBNull.Value
                        ? Convert.ToInt32(row["TotalQuantity"])
                        : 0
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }

        //public async Task<ResultVM> GetTop10Products(string? branchId, SqlConnection conn, SqlTransaction transaction)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            await conn.OpenAsync();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //                SELECT TOP 10
        //                S.ProductId,
        //                P.Name AS ProductName,
        //                SUM(S.Quantity) AS TotalQuantity,
        //                AVG(S.UnitRate) AS AverageUnitRate,
        //                SUM(S.Quantity * S.UnitRate) AS TotalSaleValue
        //            FROM SaleDetails S
        //            LEFT OUTER JOIN Products P ON S.ProductId = P.Id
        //            WHERE S.BranchId = @BranchId
        //            GROUP BY S.ProductId, P.Name
        //            ORDER BY TotalSaleValue DESC;
        //             ";

        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            // Assign transaction if exists
        //            if (transaction != null)
        //            {
        //                cmd.Transaction = transaction;
        //            }

        //            cmd.Parameters.AddWithValue("@BranchId", branchId);

        //            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
        //            {
        //                adapter.Fill(dataTable);
        //            }
        //        }

        //        var modelList = dataTable.AsEnumerable().Select(row => new ProductSaleModel
        //        {
        //            ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt32(row["ProductId"]) : 0,

        //            ProductName = row["ProductName"]?.ToString(),
        //            TotalSaleValue = row["TotalSaleValue"] != DBNull.Value
        //                ? Convert.ToDecimal(row["TotalSaleValue"])
        //                : 0,
        //            AverageUnitRate = row["AverageUnitRate"] != DBNull.Value
        //                ? Convert.ToInt32(row["AverageUnitRate"])
        //                : 0,
        //            TotalQuantity = row["TotalQuantity"] != DBNull.Value
        //                ? Convert.ToInt32(row["TotalQuantity"])
        //                : 0
        //        }).ToList();

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}

        //public async Task<ResultVM> GetBottom10Products(string? branchId, SqlConnection conn, SqlTransaction transaction)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            await conn.OpenAsync();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //                SELECT TOP 10
        //                S.ProductId,
        //                P.Name AS ProductName,
        //                SUM(S.Quantity) AS TotalQuantity,
        //                AVG(S.UnitRate) AS AverageUnitRate,
        //                SUM(S.Quantity * S.UnitRate) AS TotalSaleValue
        //            FROM SaleDetails S
        //            LEFT OUTER JOIN Products P ON S.ProductId = P.Id
        //            WHERE S.BranchId = @BranchId
        //            GROUP BY S.ProductId, P.Name
        //            ORDER BY TotalSaleValue ASC;
        //             ";

        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            // Assign transaction if exists
        //            if (transaction != null)
        //            {
        //                cmd.Transaction = transaction;
        //            }

        //            cmd.Parameters.AddWithValue("@BranchId", branchId);

        //            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
        //            {
        //                adapter.Fill(dataTable);
        //            }
        //        }

        //        var modelList = dataTable.AsEnumerable().Select(row => new ProductSaleModel
        //        {
        //            ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt32(row["ProductId"]) : 0,

        //            ProductName = row["ProductName"]?.ToString(),
        //            TotalSaleValue = row["TotalSaleValue"] != DBNull.Value
        //                ? Convert.ToDecimal(row["TotalSaleValue"])
        //                : 0,
        //            AverageUnitRate = row["AverageUnitRate"] != DBNull.Value
        //                ? Convert.ToInt32(row["AverageUnitRate"])
        //                : 0,
        //            TotalQuantity = row["TotalQuantity"] != DBNull.Value
        //                ? Convert.ToInt32(row["TotalQuantity"])
        //                : 0
        //        }).ToList();

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}

        //public async Task<ResultVM> GetTop10SalePersons(string? branchId, SqlConnection conn, SqlTransaction transaction)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            await conn.OpenAsync();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //                SELECT TOP 10
        //                S.SalePersonId,
        //                SP.Name AS SalePersonName,
        //                FORMAT(SUM(S.GrandTotalAmount), 'N2') AS GrandTotalAmount,
        //                SUM(SD.Quantity) AS TotalQuantity
        //            FROM Sales S
        //            LEFT OUTER JOIN SalesPersons SP ON S.SalePersonId = SP.Id
        //            LEFT OUTER JOIN SaleDetails SD ON S.Id = SD.SaleId
        //            WHERE S.BranchId = @BranchId 
        //            AND S.CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- First day of the current month
        //            AND S.CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) -- First day of next month
        //            GROUP BY S.SalePersonId, SP.Name
        //            ORDER BY SUM(S.GrandTotalAmount) DESC;
        //             ";

        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            // Assign transaction if exists
        //            if (transaction != null)
        //            {
        //                cmd.Transaction = transaction;
        //            }

        //            cmd.Parameters.AddWithValue("@BranchId", branchId);

        //            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
        //            {
        //                adapter.Fill(dataTable);
        //            }
        //        }

        //        var modelList = dataTable.AsEnumerable().Select(row => new SalePersonDataModel
        //        {
        //            SalePersonId = row["SalePersonId"] != DBNull.Value ? Convert.ToInt32(row["SalePersonId"]) : 0,

        //            SalePersonName = row["SalePersonName"]?.ToString(),
        //            GrandTotalAmount = row["GrandTotalAmount"] != DBNull.Value
        //                ? Convert.ToDecimal(row["GrandTotalAmount"])
        //                : 0,
        //            TotalQuantity = row["TotalQuantity"] != DBNull.Value
        //                ? Convert.ToInt32(row["TotalQuantity"])
        //                : 0
        //        }).ToList();

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}

        //public async Task<ResultVM> GetBottom10SalePersons(string? branchId, SqlConnection conn, SqlTransaction transaction)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            await conn.OpenAsync();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //                SELECT TOP 10
        //                S.SalePersonId,
        //                SP.Name AS SalePersonName,
        //                FORMAT(SUM(S.GrandTotalAmount), 'N2') AS GrandTotalAmount,
        //                SUM(SD.Quantity) AS TotalQuantity
        //            FROM Sales S
        //            LEFT OUTER JOIN SalesPersons SP ON S.SalePersonId = SP.Id
        //            LEFT OUTER JOIN SaleDetails SD ON S.Id = SD.SaleId
        //            WHERE S.BranchId = @BranchId 
        //            AND S.CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)  -- First day of the current month
        //            AND S.CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) -- First day of next month
        //            GROUP BY S.SalePersonId, SP.Name
        //            ORDER BY SUM(S.GrandTotalAmount) ASC;
        //             ";

        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            // Assign transaction if exists
        //            if (transaction != null)
        //            {
        //                cmd.Transaction = transaction;
        //            }

        //            cmd.Parameters.AddWithValue("@BranchId", branchId);

        //            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
        //            {
        //                adapter.Fill(dataTable);
        //            }
        //        }

        //        var modelList = dataTable.AsEnumerable().Select(row => new SalePersonDataModel
        //        {
        //            SalePersonId = row["SalePersonId"] != DBNull.Value ? Convert.ToInt32(row["SalePersonId"]) : 0,

        //            SalePersonName = row["SalePersonName"]?.ToString(),
        //            GrandTotalAmount = row["GrandTotalAmount"] != DBNull.Value
        //                ? Convert.ToDecimal(row["GrandTotalAmount"])
        //                : 0,
        //            TotalQuantity = row["TotalQuantity"] != DBNull.Value
        //                ? Convert.ToInt32(row["TotalQuantity"])
        //                : 0
        //        }).ToList();

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}

        public async Task<ResultVM> GetOrderPurchasePOReturnData(string? branchId, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
                        SELECT 
                        (SELECT COUNT(Id) 
                         FROM PurchaseOrders 
                         WHERE IsCompleted = 1 
                         AND BranchId = @BranchId 
                         AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                         AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
                        ) AS Ordered,
    
                        (SELECT COUNT(Id) 
                         FROM Purchases 
                         WHERE IsCompleted = 1 
	                     AND BranchId = @BranchId
                         AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) 
                         AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
                        ) AS Purchase,
    
                        (SELECT COUNT(Id) 
                         FROM PurchaseReturns 
                         WHERE IsPost = 1
	                     AND BranchId = @BranchId
                         AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                         AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
                        ) AS POReturn;
                     ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Assign transaction if exists
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }

                    cmd.Parameters.AddWithValue("@BranchId", branchId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseDataModel
                {
                    Ordered = row["Ordered"] != DBNull.Value ? Convert.ToInt32(row["Ordered"]) : 0,
                    Purchase = row["Purchase"] != DBNull.Value ? Convert.ToInt32(row["Purchase"]) : 0,
                    POReturn = row["POReturn"] != DBNull.Value ? Convert.ToInt32(row["POReturn"]) : 0
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }

        //public async Task<ResultVM> GetSalesData(string? branchId, SqlConnection conn, SqlTransaction transaction)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            await conn.OpenAsync();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //                SELECT 
        //                (SELECT COUNT(Id) FROM SaleOrders WHERE 
	       //             IsCompleted = 0
	       //             AND BranchId = @BranchId
	       //             AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
	       //             AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))) AS SaleOrdered,
        //                (SELECT COUNT(Id) FROM SaleDeleveries WHERE IsCompleted = 0
	       //             AND BranchId = @BranchId
	       //             AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
	       //             AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
	       //             ) AS SaleDelivered,
        //                (SELECT COUNT(Id) FROM SaleDeleveryReturns WHERE IsCompleted = 0
	       //             AND BranchId = @BranchId
	       //             AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
	       //             AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
	       //             ) AS SaleDeliveryReturn,
        //                (SELECT COUNT(Id) FROM Sales WHERE IsPost = 0
	       //             AND BranchId = @BranchId
	       //             AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
	       //             AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))) AS Sales,
        //                (SELECT COUNT(Id) FROM SaleReturns WHERE IsPost = 0
	       //             AND BranchId = @BranchId
	       //             AND CreatedOn >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
	       //             AND CreatedOn < DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))) AS SaleReturn;
        //             ";

        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            // Assign transaction if exists
        //            if (transaction != null)
        //            {
        //                cmd.Transaction = transaction;
        //            }

        //            cmd.Parameters.AddWithValue("@BranchId", branchId);

        //            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
        //            {
        //                adapter.Fill(dataTable);
        //            }
        //        }

        //        var modelList = dataTable.AsEnumerable().Select(row => new SalesDataModel
        //        {
        //            SaleOrdered = row["SaleOrdered"] != DBNull.Value ? Convert.ToInt32(row["SaleOrdered"]) : 0,
        //            SaleDelivered = row["SaleDelivered"] != DBNull.Value ? Convert.ToInt32(row["SaleDelivered"]) : 0,
        //            SaleDeliveryReturn = row["SaleDeliveryReturn"] != DBNull.Value ? Convert.ToInt32(row["SaleDeliveryReturn"]) : 0,
        //            Sales = row["Sales"] != DBNull.Value ? Convert.ToInt32(row["Sales"]) : 0,
        //            SaleReturn = row["SaleReturn"] != DBNull.Value ? Convert.ToInt32(row["SaleReturn"]) : 0
        //        }).ToList();

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}

        public async Task<ResultVM> GetPendingSales(string? branchId, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
                        SELECT 
                        COUNT(SaleDeleveries.Id) AS OrderedButNotDelivered,
                        C.Name AS CustomerName
                        FROM SaleDeleveries
                        LEFT OUTER JOIN Customers C 
                            ON SaleDeleveries.CustomerId = C.Id
                        WHERE SaleDeleveries.IsCompleted = 0
                        AND SaleDeleveries.BranchId = @BranchId
                        GROUP BY SaleDeleveries.CustomerId, C.Name;
                     ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Assign transaction if exists
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }

                    cmd.Parameters.AddWithValue("@BranchId", branchId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                var modelList = dataTable.AsEnumerable().Select(row => new PendingSalesDataModel
                {
                    CustomerName = row["CustomerName"]?.ToString(),
                    OrderedButNotDelivered = row["OrderedButNotDelivered"] != DBNull.Value ? Convert.ToInt32(row["OrderedButNotDelivered"]) : 0
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }


        //public async Task<ResultVM> ListRouteBySalePersonAndBranch(int salePersonId, int branchId, SqlConnection conn, SqlTransaction transaction)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        // If no connection is passed, create a new one
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        // SQL query to get the customers based on SalePersonId and BranchId
        //        string query = @"
        //     select 

        //             ISNULL(H.Id, 0) AS Id,
        //             ISNULL(H.SalePersonId, 0) AS SalePersonId,
        //             ISNULL(SP.Name, '') AS SalePersonName,
        //             ISNULL(H.BranchId, 0) AS BranchId,
        //             ISNULL(B.Name, '') AS BranchName,
        //             ISNULL(H.RouteId, 0) AS RouteId,
        //             ISNULL(R.Name, '') AS Name

        //            FROM SalePersonRoutes H
        //             LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
        //             LEFT OUTER JOIN BranchProfiles B ON H.BranchId = B.Id
        //             LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        //            WHERE sp.BranchId = @BranchId
        //            AND H.SalePersonId = @SalePersonId
        //            AND H.IsArchive=0    
        //";

        //        // Create the SQL command and set parameters
        //        SqlCommand cmd = new SqlCommand(query, conn, transaction);
        //        cmd.Parameters.AddWithValue("@SalePersonId", salePersonId);
        //        cmd.Parameters.AddWithValue("@BranchId", branchId);

        //        // Create a data adapter to fill the DataTable
        //        SqlDataAdapter objComm = new SqlDataAdapter(cmd);
        //        objComm.Fill(dataTable);

        //        // Convert the DataTable rows to a list of CustomerDataVM objects
        //        var modelList = dataTable.AsEnumerable().Select(row => new SalePersonRouteVM
        //        {
        //            Id = row.Field<int>("Id"),
        //            BranchId = row.Field<int>("BranchId"),
        //            SalePersonId = row.Field<int>("SalePersonId"),
        //            RouteId = row.Field<int>("RouteId"),
        //            BranchName = row.Field<string>("BranchName"),
        //            SalePersonName = row.Field<string>("SalePersonName"),
        //            Name = row.Field<string>("Name")

        //        }).ToList();

        //        // If there are any customers, set the status to Success
        //        if (modelList.Any())
        //        {
        //            result.Status = "Success";
        //            result.Message = "Data retrieved successfully.";
        //            result.DataVM = modelList;
        //        }
        //        else
        //        {
        //            result.Status = "Fail";
        //            result.Message = "No customers found for the given SalePersonId and BranchId.";
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and return the error message
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //        return result;
        //    }
        //    finally
        //    {
        //        // Close the connection if it was opened in this method
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}


        //public async Task<ResultVM> GetCampaignList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //    SELECT DISTINCT
        //      ISNULL(H.Id,0)	Id,
        //      ISNULL(H.Code,'') Code  ,               
        //      ISNULL(H.Name,'') Name


        //      FROM Campaigns H
        //      WHERE 1 =1";


        //        query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //        SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new CampaignVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            Code = row["Code"]?.ToString(),
        //            Name = row["Name"]?.ToString(),


        //        }).ToList();


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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

        public async Task<ResultVM> PaymentTypeList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                ISNULL(H.Name, '') AS Name,
                ISNULL(H.IsArchive, 0) AS IsArchive,
                ISNULL(H.IsActive, 0) AS IsActive,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                ISNULL(H.CreatedFrom, '') AS CreatedFrom,
                ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
            FROM 
                PaymentTypes H
            WHERE 1 = 1 ";


                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new PaymentTypeVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    IsArchive = Convert.ToBoolean(row["IsArchive"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedBy = row["CreatedBy"]?.ToString(),
                    CreatedOn = row["CreatedOn"]?.ToString(),
                    LastModifiedBy = row["LastModifiedBy"]?.ToString(),
                    LastModifiedOn = row["LastModifiedOn"]?.ToString(),
                    CreatedFrom = row["CreatedFrom"]?.ToString(),
                    LastUpdateFrom = row["LastUpdateFrom"]?.ToString(),


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

        //public async Task<ResultVM> SaleDeleveryList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }
        //        string sqlQuery = @"
        //        SELECT DISTINCT

        //         ISNULL(H.Id, 0) Id
        //        ,ISNULL(H.Code, '') Code
        //        , ISNULL(H.DeliveryPersonId, 0) DeliveryPersonId
                



        //        FROM SaleDeleveries H

        //        WHERE 1 = 1

        //                     ";


        //        sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

        //        SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new SaleDeliveryVM
        //        {
        //            Id = Convert.ToInt32(row["Id"]),
        //            DeliveryPersonId = Convert.ToInt32(row["DeliveryPersonId"]),



        //        }).ToList();


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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

//        public async Task<ResultVM> GetSaleDeleveryModal(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

               




//                string query = @"
//SELECT 
//ISNULL(P.Id,0)SaleDeleverieId , 
//ISNULL(P.Code,'') Code,
//ISNULL(P.SalePersonId,0)SalePersonId , 
//ISNULL(P.DeliveryPersonId,0)DeliveryPersonId , 
//ISNULL(P.DriverPersonId,0)DriverPersonId ,
//ISNULL(P.DeliveryAddress,'') DeliveryAddress,
//ISNULL(FORMAT(P.DeliveryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') DeliveryDate,
//ISNULL(P.RestAmount,0) GrandTotalAmount,
//ISNULL(P.PaidAmount,0) PaidAmount
 

//FROM SaleDeleveries P

//WHERE  1 = 1 
//and isnull(p.ispost,0)='1'
//and isnull(p.Processed,0)='0'


//";
//                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
//                {
//                    query += @"  and P.BranchId=@BranchId";
//                }
//                if (vm != null && vm.CustomerId!=0)
//                {
//                    query += @"  and P.CustomerId=@CustomerId" ;
//                }


//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
//                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
//                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

//                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
//                }
//                if (vm != null && vm.CustomerId != 0)
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
//                }
//                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
//                }

//                objComm.Fill(dataTable);

//                var modelList = dataTable.AsEnumerable().Select(row => new SaleDeliveryVM
//                {
//                    Id = row.Field<int>("SaleDeleverieId"),
//                    Code = row.Field<string>("Code"),
//                    SalePersonId = row.Field<int>("SalePersonId"),
//                    DeliveryPersonId = row.Field<int>("DeliveryPersonId"),
//                    DriverPersonId = row.Field<int>("DriverPersonId"),
//                    DeliveryAddress = row.Field<string>("DeliveryAddress"),
//                    DeliveryDate = row.Field<string>("DeliveryDate"),
//                    GrandTotalAmount = row.Field<decimal>("GrandTotalAmount"),
//                    PaidAmount = row.Field<decimal>("PaidAmount"),

//                }).ToList();


//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = modelList;
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

        public async Task<ResultVM> getSaleDeleveryModalCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

                //Add

                SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
                checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
                object res = checkCommand.ExecuteScalar();
                string customerCategory = res != null ? res.ToString() : "0";

                //End


                string query = @"
SELECT

COALESCE(COUNT(P.Id), 0) AS FilteredCount

FROM SaleDeleveries P
WHERE 1 = 1 
";



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





        public async Task<ResultVM> GetProductStockQuantity(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

                SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
                checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
                object res = checkCommand.ExecuteScalar();
                string customerCategory = res != null ? res.ToString() : "0";


                string query = @"


SELECT 
    ISNULL(P.Id, 0) AS ProductId, 
    ISNULL(P.Name, '') AS ProductName,
    ISNULL(P.BanglaName, '') AS BanglaName, 
    ISNULL(P.Code, '') AS ProductCode, 
    ISNULL(P.HSCodeNo, '') AS HSCodeNo,
    ISNULL(P.ImagePath, '') AS ImagePath,
    ISNULL(P.ProductGroupId, 0) AS ProductGroupId,
    ISNULL(PG.Name, '') AS ProductGroupName,
    ISNULL(P.UOMId, 0) AS UOMId,
    ISNULL(UOM.Name, '') AS UOMName,
    ISNULL(PBH.CostPrice, 0) AS CostPrice, 
    ISNULL(PBH.SalesPrice, 0) AS SalesPrice, 
    ISNULL(PBH.PurchasePrice, 0) AS PurchasePrice, 
    ISNULL(PBH.SD, 0) AS SD, 
    ISNULL(PBH.VATRate, 0) AS VATRate, 
    ISNULL(stock.Quantity, 0) AS QuantityInHand,
    P.CtnSize AS UOMName, 
    P.CtnSizeFactor AS UOMConversion,
    CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status

FROM Products P
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId
LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
LEFT OUTER JOIN (
    SELECT 
        BranchId, 
        ProductId, 
        SUM(Quantity) AS Quantity 
    FROM (
        SELECT 
            BranchId,
            ProductId,
            SUM(CASE 
                    WHEN Type = 'Purchase' THEN Quantity 
                    WHEN Type = 'Sales' THEN -Quantity 
                    ELSE 0 
                END) AS Quantity
        FROM DayEndDetails
        WHERE BranchId = @BranchId AND Date <= @FromDate
        GROUP BY BranchId, ProductId

        UNION ALL

        SELECT 
            SaleOrders.BranchId,
            ProductId,
            SUM(Quantity) * -1 AS Quantity
        FROM SaleOrderDetails
        LEFT JOIN SaleOrders ON SaleOrders.Id = SaleOrderDetails.SaleOrderId
        WHERE SaleOrders.BranchId = @BranchId AND SaleOrders.InvoiceDateTime >= @FromDate
        GROUP BY SaleOrders.BranchId, ProductId
    ) AS Result
    GROUP BY BranchId, ProductId
) stock ON P.Id = stock.ProductId 

WHERE 
    P.Id = @ProductId AND
    PBH.PriceCategory = @PriceCategory AND 
    PBH.BranchId = @BranchId AND 
    CAST(PBH.EffectDate AS DATE) <= @FromDate AND  
    P.IsActive = 1


";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.PriceCategory = @PriceCategory AND PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                //query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
                //query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

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
                if (customerCategory != null)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@PriceCategory", customerCategory);
                }
                if (vm != null && vm.ProductId != 0)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new ProductDataVM
                {
                    ProductId = row.Field<int>("ProductId"),
                    ProductName = row.Field<string>("ProductName"),
                    BanglaName = row.Field<string>("BanglaName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    HSCodeNo = row.Field<string>("HSCodeNo"),
                    ImagePath = row.Field<string>("ImagePath"),
                    ProductGroupId = row.Field<int>("ProductGroupId"),
                    SDRate = row.Field<decimal>("SD"),
                    VATRate = row.Field<decimal>("VATRate"),
                    CostPrice = row.Field<decimal>("CostPrice"),
                    SalesPrice = row.Field<decimal>("SalesPrice"),
                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    UOMId = row.Field<int>("UOMId"),
                    QuantityInHand = row.Field<decimal>("QuantityInHand"),
                    UOMName = row.Field<string>("UOMName"),
                    UOMConversion = row.Field<string>("UOMConversion"),
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
        public async Task<ResultVM> BranchProfileList(
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

                string query = @"
            SELECT DISTINCT
                 ISNULL(H.Code,'') Code,
                 ISNULL(H.Id,0) Id,
                 ISNULL(H.Name,'') Name,
                 ISNULL(H.Email,'') Email
            FROM BranchProfiles H
            WHERE H.IsActive = 1 
              AND H.Id NOT IN (SELECT BranchId FROM Campaigns WHERE Id = @CampaignId)";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // Add CampaignId from conditionalValues (example assumes it's the first value)
                if (conditionalValues != null && conditionalValues.Length > 0)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CampaignId", conditionalValues[0]);
                }
                else
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CampaignId", 0); // default fallback
                }

                // Apply other conditions if provided
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





        //     public async Task<ResultVM> BranchProfileList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //     {
        //         bool isNewConnection = false;
        //         DataTable dataTable = new DataTable();
        //         ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //         try
        //         {
        //             if (conn == null)
        //             {
        //                 conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //                 conn.Open();
        //                 isNewConnection = true;
        //             }

        //             string query = @"            

        //SELECT DISTINCT
        // ISNULL(H.Code,'') Code
        //,ISNULL(H.Id,0) Id
        //,ISNULL(H.Name,'') Name
        //   ,ISNULL(H.Email,'') Email


        //FROM BranchProfiles H

        //WHERE H.IsActive=1 AND H.Id Not In (Select BranchId from Campaigns Where Id = conditionalValues.id) ";


        //             query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //             SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //             // SET additional conditions param
        //             objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



        //             objComm.Fill(dataTable);

        //             var modelList = dataTable.AsEnumerable().Select(row => new BranchProfileVM
        //             {
        //                 Id = Convert.ToInt32(row["Id"]),
        //                 Name = row["Name"]?.ToString(),
        //                 Code = row["Code"]?.ToString(),
        //                 Email = row["Email"]?.ToString(),


        //             }).ToList();


        //             result.Status = "Success";
        //             result.Message = "Data retrieved successfully.";
        //             result.DataVM = modelList;
        //             return result;
        //         }
        //         catch (Exception ex)
        //         {
        //             result.ExMessage = ex.Message;
        //             result.Message = ex.Message;
        //             return result;
        //         }
        //         finally
        //         {
        //             if (isNewConnection && conn != null)
        //             {
        //                 conn.Close();
        //             }
        //         }
        //     }
    }

}
