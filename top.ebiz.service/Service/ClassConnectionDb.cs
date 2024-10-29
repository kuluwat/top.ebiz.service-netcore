using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace top.ebiz.service.Service
{
    public class ClassConnectionDb : IDisposable
    {
        static public string ConnectionString()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["eBizConnection"] ?? "";
        }
        public OracleConnection conn;
        public OracleTransaction trans;
        public void OpenConnection()
        {
            if (conn == null)
            {
                conn = new OracleConnection(ConnectionString());
            }

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
        }
        public void CloseConnection()
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
                conn.Dispose();
            }
        }
        public void BeginTransaction()
        {
            if (trans == null)
            {
                trans = conn.BeginTransaction();
            }
        }
        public void Commit()
        {
            if (trans != null)
            {
                trans.Commit();
            }
        }
        public void Rollback()
        {
            if (trans != null)
            {
                trans.Rollback();
            }
        }
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    trans?.Dispose();
                    conn?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        //static public bool IsAuthorizedRole(string userName, string roleType)
        static public bool IsAuthorizedRole()
        {
            return true;
            //// ตรวจสอบว่า userName และ roleType มีค่าไม่เป็นค่าว่างหรือไม่
            //if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(roleType))
            //{
            //    return false; // ถ้า userName หรือ roleType เป็นค่าว่าง ให้ถือว่าไม่ได้รับอนุญาต
            //}

            //// ตรวจสอบสิทธิ์บทบาท (เฉพาะ admin, approver, employee เท่านั้นที่อนุญาต ยกเว้น reviewer)
            //var allowedRoles = new List<string> { "admin", "approver", "employee" }; // รายการบทบาทที่อนุญาต 
            //if (allowedRoles.Contains(roleType))
            //{
            //    return true; // หาก roleType อยู่ในรายการที่อนุญาต ให้อนุญาต
            //}
            //else
            //{
            //    return false; // หากไม่อยู่ในรายการที่อนุญาต ให้ปฏิเสธ
            //}
        }

        public OracleParameter ConvertTypeParameter(string paramName, object value, string type = "char", int defLength = 4000)
        {
            OracleParameter param = new OracleParameter();
            param.ParameterName = paramName;

            switch (type.ToLower())
            {
                case "char":
                    param.OracleDbType = OracleDbType.Char;
                    param.Size = value != null ? value.ToString().Length : defLength; // กำหนดขนาด
                    param.Value = value ?? DBNull.Value; // ถ้าไม่มีค่าให้เป็น DBNull
                    break;

                case "int":
                    param.OracleDbType = OracleDbType.Int32;
                    param.Value = value != null && int.TryParse(value.ToString(), out int intValue) ? intValue : DBNull.Value;
                    break;

                case "number":
                    param.OracleDbType = OracleDbType.Decimal;
                    param.Value = value != null && decimal.TryParse(value.ToString(), out decimal decimalValue) ? decimalValue : DBNull.Value;
                    break;

                case "date":
                    param.OracleDbType = OracleDbType.Date;
                    param.Value = value != null && DateTime.TryParse(value.ToString(), out DateTime dateValue) ? dateValue : DBNull.Value;
                    break;

                default:
                    param.OracleDbType = OracleDbType.Varchar2; // Default เป็น string
                    param.Value = value ?? DBNull.Value;
                    break;
            }

            return param;
        }
        public DataSet ExecuteAdapter(OracleCommand cmd)
        {
            if (cmd.CommandType != CommandType.StoredProcedure)
            {
                cmd.CommandType = CommandType.Text;
            }
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    foreach (DataColumn column in ds.Tables[0].Columns)
                    {
                        column.ColumnName = column.ColumnName.ToLower();
                    }
                }
            }
            return ds;
        }

        //public string ExecuteNonQuerySQL(OracleCommand sqlCommand, string user_name, string role_type)
        public string ExecuteNonQuerySQL(OracleCommand sqlCommand)
        {
            //if (!ClassLogin.IsAuthorized(user_name))
            //{
            //    return "User is not authorized to perform this action.";
            //}

            string ret = "";
            string query = sqlCommand.CommandText;
            try
            {
                if (sqlCommand.CommandType != CommandType.StoredProcedure)
                {
                    sqlCommand.CommandType = CommandType.Text;
                }
                //if (!ClassLogin.IsAuthorizedRole(user_name, role_type))
                //{
                //    return "User is not authorized to perform this action.";
                //}
                //else
                //{ 
                sqlCommand.ExecuteNonQuery();

                ret = "true";
                //}

            }
            catch (Exception ex)
            {
                ret = ex.ToString();
                // query error 
                foreach (OracleParameter p in sqlCommand.Parameters)
                {
                    string value = p.Value == null ? "NULL" : p.Value?.ToString() ?? "";
                    if (p.OracleDbType == OracleDbType.Varchar2 || p.OracleDbType == OracleDbType.Char)
                    {
                        value = $"'{value}'";
                    }
                    query = query.Replace(p.ParameterName, value);
                }
                //// Write the query to a text file
                //System.IO.File.WriteAllText("executed_query.txt", query);
            }
            return ret;
        }


        public static OracleParameter CreateSqlParameter(string parameterName, OracleDbType dbType, object value, int length = 0)
        {
            if (value == null)
            {
                return new OracleParameter(parameterName, dbType) { Value = DBNull.Value };
            }

            if (dbType == OracleDbType.Int64 && int.TryParse(value.ToString(), out int intValue))
            {
                return new OracleParameter(parameterName, dbType) { Value = intValue };
            }

            if (dbType == OracleDbType.Decimal && decimal.TryParse(value.ToString(), out decimal decimalValue))
            {
                return new OracleParameter(parameterName, dbType) { Value = decimalValue };
            }

            if (dbType == OracleDbType.Date && DateTime.TryParse(value.ToString(), out DateTime dateTimeValue))
            {
                return new OracleParameter(parameterName, dbType) { Value = dateTimeValue };
            }

            if (dbType == OracleDbType.Varchar2)
            {
                if (length > 0)
                {
                    return new OracleParameter(parameterName, dbType, length) { Value = value.ToString() };
                }
                else
                {
                    return new OracleParameter(parameterName, dbType) { Value = value.ToString() };
                }
            }

            // Default case for other data types
            return new OracleParameter(parameterName, dbType) { Value = value.ToString() };
        }

    }

}
