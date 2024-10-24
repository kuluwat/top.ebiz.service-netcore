using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace top.ebiz.service.Service.Create_trip
{
    public class cls_connection_ebiz
    {
        OracleConnection conn;
        OracleDataAdapter da;
        OracleCommand comm;
        OracleTransaction trans;

        private DataSet ds;

        string ConnStr = System.Configuration.ConfigurationManager.AppSettings["ConnString"].ToString();

        public void OpenConnection()
        {
            if (conn == null)
            {
                conn = new OracleConnection(ConnStr);

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
        public string ExecuteNonQuery(string SqlStatement)
        {
            string ret = "";
            try
            {
                comm = new OracleCommand(SqlStatement, conn);
                if (trans != null)
                {
                    comm.Connection = conn;
                    comm.Transaction = trans;
                }

                comm.ExecuteNonQuery();

                ret = "True";
            }
            catch (Exception ex)
            {
                ret = ex.ToString();
            }
            return ret;
        }
        public DataSet ExecuteAdapter(string SqlStatement)
        {
            da = new OracleDataAdapter(SqlStatement, conn);
            ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        public void BeginTransaction()
        {
            if (trans == null)
            {
                trans = conn.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            if (trans != null)
            {
                trans.Commit();
            }
        }

        public void RollbackTransaction()
        {
            if (trans != null)
            {
                trans.Rollback();
            }
        }
        public string CurrentUserName()
        {
            string CurrentUserName = HttpContext.Current.User.Identity.Name; // Gives actual user logged on (as seen in <ASP:Login />) 
            try
            {
                return CurrentUserName.Replace(@"THAIOILNT\", "");
            }
            catch { return ""; }
        }


        #region function 
        public string ChkSqlNum(object Str, string nType)
        {
            string xNum = "NULL";
            if (Str == null || Convert.IsDBNull(Str))
            {
                return "NULL";
            }
            else if (Str == "NULL")
            {
                return "NULL";
            }
            else
            {
                try
                {
                    if (nType == "N")
                    {
                        xNum = Convert.ToString(Convert.ToInt64(Str.ToString()));
                    }
                    else if (nType == "D")
                    {
                        xNum = Convert.ToString(Convert.ToDouble(Str.ToString()));
                    }
                }
                catch
                {
                    return "NULL";
                }
            }
            return xNum;
        }
        public string ChkSqlNum(object Str, string nType, int iLength)
        {
            string xNum = "NULL";
            if (Str == null || Convert.IsDBNull(Str))
            {
                return "NULL";
            }
            else if (Str == "NULL")
            {
                return "NULL";
            }
            else
            {
                try
                {
                    if (nType == "N")
                    {
                        xNum = Convert.ToString(Convert.ToInt64(Str.ToString()));
                    }
                    else if (nType == "D")
                    {
                        if (iLength == 0)
                        {
                            xNum = Convert.ToString(Convert.ToDouble(Str.ToString()).ToString("##0"));
                        }
                        else if (iLength == 1)
                        {
                            xNum = Convert.ToString(Convert.ToDouble(Str.ToString()).ToString("##0.0"));
                        }
                        else if (iLength == 2)
                        {
                            xNum = Convert.ToString(Convert.ToDouble(Str.ToString()).ToString("##0.00"));
                        }
                        else if (iLength == 3)
                        {
                            xNum = Convert.ToString(Convert.ToDouble(Str.ToString()).ToString("##0.000"));
                        }
                        else if (iLength == 4)
                        {
                            xNum = Convert.ToString(Convert.ToDouble(Str.ToString()).ToString("##0.0000"));
                        }
                        else
                        {
                            xNum = Convert.ToString(Convert.ToDouble(Str.ToString()));
                        }
                    }
                }
                catch
                {
                    return "NULL";
                }
            }
            return xNum;
        }
        public string ChkSqlStr(object Str, int Length)
        {
            //วิธีที่ 1 --> แทนที่ ' ด้วย ช่องว่าง 1 ช่อง --> " " ทำให้ ' ใน base หายไป
            //วิธีที่ 2 --> แทนที่ ' ด้วย ''         --> Chr(39) & Chr(39) ทำให้ ' ใน base ยังอยู่ 

            //Str = "เลี้ยงตอบแทน' บ.Cyberouis, XX'XX'xxx'xxx"

            string Str1;

            if (Str == null || Convert.IsDBNull(Str))
            {
                return "null";
            }

            if (Str.ToString().ToLower() == "null")
            {
                return "null";
            }

            if (Str.ToString().Trim() == "")
            {
                return "null";
            }

            Str1 = Str.ToString();

            //วิธีที่ 1
            //Str1 = Replace(Str1, Chr(39), " ")

            //วิธีที่ 2
            //Str1 = Replace(Str1, Chr(39), Chr(39) & Chr(39))
            Str1 = Str1.Replace("'", "''");

            if (Str1.ToString().Length >= Length)
            {
                return "'" + Str1.ToString().Substring(0, Length) + "'";
            }
            else
            {
                return "'" + Str1.ToString().Trim() + "'";
            }
        }
        public string ChkSqlStrSpe(object Str, int Length)
        {
            //วิธีที่ 1 --> แทนที่ ' ด้วย ช่องว่าง 1 ช่อง --> " " ทำให้ ' ใน base หายไป
            //วิธีที่ 2 --> แทนที่ ' ด้วย ''         --> Chr(39) & Chr(39) ทำให้ ' ใน base ยังอยู่ 

            //Str = "เลี้ยงตอบแทน' บ.Cyberouis, XX'XX'xxx'xxx"

            string Str1;

            if (Str == null || Convert.IsDBNull(Str))
            {
                return "null";
            }

            if (Str.ToString().ToLower() == "null")
            {
                return "null";
            }

            if (Str.ToString().Trim() == "")
            {
                return "null";
            }

            Str1 = Str.ToString();

            //วิธีที่ 1
            //Str1 = Replace(Str1, Chr(39), " ")

            //วิธีที่ 2
            //Str1 = Replace(Str1, Chr(39), Chr(39) & Chr(39))
            Str1 = Str1.Replace("'", "''");


            Str1 = Str1.Replace("  ", "");
            Str1 = Str1.Replace("   ", "");
            Str1 = Str1.Replace(@"
            ", "");

            if (Str1.ToString().Length >= Length)
            {
                return "'" + Str1.ToString().Substring(0, Length) + "'";
            }
            else
            {
                return "'" + Str1.ToString().Trim() + "'";
            }
        }
        public string ChkSqlDateTime(object sDate)
        {
            try
            {
                int dd;
                int mm;
                int yyyy;

                if (Convert.IsDBNull(sDate))
                {
                    return "NULL";
                }
                else if (sDate.ToString().Replace(" ", "") == "")
                {
                    return "NULL";
                }
                else
                {
                    string xDate = "";
                    DateTime tsDate = new DateTime();
                    System.Globalization.CultureInfo TmpConvert = new System.Globalization.CultureInfo("en-US");

                    // กรณีที่เป็น Date จาหหน้าจอ ให้เป็น dd/MM/yyyy
                    string[] xDate1;
                    xDate1 = sDate.ToString().Split('/');
                    try
                    {
                        tsDate = new DateTime(Convert.ToInt16(xDate1[2]), Convert.ToInt16(xDate1[1]), Convert.ToInt16(xDate1[0]));
                    }
                    catch
                    {
                        // ยังไม่ได้ check ละเอียดนะ
                        try
                        { // กรณีที่เป็น type date เฉพาะ rmis เท่านั้น   dd/MM/yyyy
                            tsDate = new DateTime(Convert.ToInt16(xDate1[2].ToString().Substring(0, 4)), Convert.ToInt16(xDate1[1]), Convert.ToInt16(xDate1[0]));
                        }
                        catch
                        {
                            // กรณีที่เป็น type date เฉพาะ rmis เท่านั้น   MM/dd/yyyy
                            tsDate = new DateTime(Convert.ToInt16(xDate1[2].ToString().Substring(0, 4)), Convert.ToInt16(xDate1[0]), Convert.ToInt16(xDate1[1]));
                        }

                    }
                    if (tsDate.Year > 2500) { tsDate = tsDate.AddYears(-543); }
                    if (tsDate.Year < 2000) { tsDate = tsDate.AddYears(543); }
                    xDate = "to_date('" + tsDate.ToString("dd-MMM-yyyy hh:mm:ss", TmpConvert) + "','dd-Mon-yyyy hh24:mi:ss',' NLS_DATE_LANGUAGE=ENGLISH')";

                    return xDate;
                }
            }
            catch// ทดสอบถ้า debug ได้ให้เอาออก
            {

                return "NULL";
            }
        }
        public string ChkSqlDateMMDDYYYY(object sDate)
        {
            try
            {
                int dd;
                int mm;
                int yyyy;

                if (Convert.IsDBNull(sDate))
                {
                    return "NULL";
                }
                else if (sDate.ToString().Replace(" ", "") == "")
                {
                    return "NULL";
                }
                else
                {
                    string xDate = "";
                    DateTime tsDate = new DateTime();
                    System.Globalization.CultureInfo TmpConvert = new System.Globalization.CultureInfo("en-US");

                    // กรณีที่เป็น Date จาหหน้าจอ ให้เป็น MM/dd/yyyy
                    string[] xDate1;
                    xDate1 = sDate.ToString().Split('/');
                    try
                    {
                        //tsDate = new DateTime(Convert.ToInt16(xDate1[2]), Convert.ToInt16(xDate1[1]), Convert.ToInt16(xDate1[0]));
                        tsDate = new DateTime(Convert.ToInt16(xDate1[2].ToString().Substring(0, 4)), Convert.ToInt16(xDate1[0]), Convert.ToInt16(xDate1[1]));
                    }
                    catch
                    {
                        //// ยังไม่ได้ check ละเอียดนะ
                        //try
                        //{ // กรณีที่เป็น type date เฉพาะ rmis เท่านั้น   dd/MM/yyyy
                        //    tsDate = new DateTime(Convert.ToInt16((xDate1[2]).ToString().Substring(0, 4)), Convert.ToInt16(xDate1[1]), Convert.ToInt16(xDate1[0]));
                        //}
                        //catch
                        //{
                        //    // กรณีที่เป็น type date เฉพาะ rmis เท่านั้น   MM/dd/yyyy
                        //    tsDate = new DateTime(Convert.ToInt16((xDate1[2]).ToString().Substring(0, 4)), Convert.ToInt16(xDate1[0]), Convert.ToInt16(xDate1[1]));
                        //}

                    }
                    if (tsDate.Year > 2500) { tsDate = tsDate.AddYears(-543); }
                    if (tsDate.Year < 2000) { tsDate = tsDate.AddYears(543); }
                    xDate = "to_date('" + tsDate.ToString("dd-MMM-yyyy hh:mm:ss", TmpConvert) + "','dd-Mon-yyyy hh24:mi:ss',' NLS_DATE_LANGUAGE=ENGLISH')";

                    return xDate;
                }
            }
            catch// ทดสอบถ้า debug ได้ให้เอาออก
            {
                return "NULL";
            }
        }
        public string ChkSqlDateYYYYMMDD(object sDate)
        {
            //20191123 or 2019-11-23 
            try
            {
                int dd;
                int mm;
                int yyyy;

                if (Convert.IsDBNull(sDate))
                {
                    return "NULL";
                }
                else if (sDate.ToString().Replace(" ", "") == "")
                {
                    return "NULL";
                }
                else
                {
                    //sDate = sDate.ToString().Replace("-", "");
                    if (sDate.ToString().IndexOf("-") > -1)
                    {
                        string[] xsDate = sDate.ToString().Split('-');
                        if (xsDate.Length > 2)
                        {
                            sDate = xsDate[0].ToString();
                            if (xsDate[1].ToString().Length == 1) { sDate += "0"; }
                            sDate += xsDate[1].ToString();
                            if (xsDate[2].ToString().Length == 1) { sDate += "0"; }
                            sDate += xsDate[2].ToString();
                        }
                    }

                    string xDate = "";
                    DateTime tsDate = new DateTime();
                    System.Globalization.CultureInfo TmpConvert = new System.Globalization.CultureInfo("en-US");

                    // กรณีที่เป็น Date จาหหน้าจอ ให้เป็น MM/dd/yyyy 
                    try
                    {
                        tsDate = new DateTime(Convert.ToInt16(sDate.ToString().Substring(0, 4)), Convert.ToInt16(sDate.ToString().Substring(4, 2)), Convert.ToInt16(sDate.ToString().Substring(6, 2)));
                    }
                    catch
                    {

                    }
                    if (tsDate.Year > 2500) { tsDate = tsDate.AddYears(-543); }
                    if (tsDate.Year < 2000) { tsDate = tsDate.AddYears(543); }
                    xDate = "to_date('" + tsDate.ToString("dd-MMM-yyyy hh:mm:ss", TmpConvert) + "','dd-Mon-yyyy hh24:mi:ss',' NLS_DATE_LANGUAGE=ENGLISH')";

                    return xDate;
                }
            }
            catch// ทดสอบถ้า debug ได้ให้เอาออก
            {

                return "NULL";
            }
        }

        #endregion function

    }

}