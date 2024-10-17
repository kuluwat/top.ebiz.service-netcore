
using System;
using System.Data;
namespace top.ebiz.service.Service
{
    public class SAPService
    {
        #region connection  
        internal static string conn_ExecuteData(ref DataTable dtSelect, string sqlstr)
        {
            dtSelect = new DataTable();
            try
            {
                //adapter_data
                ws_conn.wsConnection conn_ws = new ws_conn.wsConnection();
                dtSelect = conn_ws.adapter_data(sqlstr);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        internal static string conn_ExecuteNonQuery(string sqlstr, Boolean type_check)
        {
            try
            {
                ws_conn.wsConnection conn_ws = new ws_conn.wsConnection();
                string ret = conn_ws.execute_data(sqlstr, type_check);
                if (ret == "") { ret = "true"; }
                return ret.ToLower();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        #endregion connection 

        public string ZTHROMB004(string data_daily)
        {
            string SAP_AppServerHost = System.Configuration.ConfigurationManager.AppSettings["SAP_AppServerHost"].ToString();
            string SAP_Client = System.Configuration.ConfigurationManager.AppSettings["SAP_Client"].ToString();
            string SAP_Username = System.Configuration.ConfigurationManager.AppSettings["SAP_Username"].ToString();
            string SAP_Password = System.Configuration.ConfigurationManager.AppSettings["SAP_Password"].ToString();

            string msg = "";

            string date_now = DateTime.Now.ToString("yyyyMMdd");
            string date_now_m = DateTime.Now.ToString("yyyy");

            try
            {
                //DevFix 20210802 กรณีบน server จะ call sap แบบ 32bit แต่ db/web จะเป็น 64bit
                //http://tbkc-dapps-05.thaioil.localnet/ebiz_ws/Table/wsConnection.asmx
                ws_conn.wsConnection conn = new ws_conn.wsConnection();


                ZTHROMB004_1.RANGE_DATE Im_Chg_Date = new ZTHROMB004_1.RANGE_DATE();
                Im_Chg_Date.Option = "BT";
                Im_Chg_Date.Sign = "I";

                if (data_daily.ToString() == "true")
                {
                    Im_Chg_Date.Low = date_now_m + "0101";
                    Im_Chg_Date.High = date_now;
                }
                else
                {
                    //ข้อมูลทั้งหมด
                    Im_Chg_Date.Low = "20100101";
                    Im_Chg_Date.High = date_now;
                }

                string Im_Infty = "";
                string Im_Subty = "";

                ZTHROMB004_1.BAPIRETURN1 Ex_Return = new ZTHROMB004_1.BAPIRETURN1();
                ZTHROMB004_1.ZESS_PA0002Table Tab_Pa0002 = new ZTHROMB004_1.ZESS_PA0002Table();
                ZTHROMB004_1.ZESS_PA0006Table Tab_Pa0006 = new ZTHROMB004_1.ZESS_PA0006Table();

                ZTHROMB004_1.ZESS_PA0019Table ZESS_PA0019 = new ZTHROMB004_1.ZESS_PA0019Table();
                ZTHROMB004_1.ZESS_PA0021Table ZESS_PA0021 = new ZTHROMB004_1.ZESS_PA0021Table();
                ZTHROMB004_1.ZESS_PA0022Table ZESS_PA0022 = new ZTHROMB004_1.ZESS_PA0022Table();
                ZTHROMB004_1.ZESS_PA0041Table ZESS_PA0041 = new ZTHROMB004_1.ZESS_PA0041Table();
                ZTHROMB004_1.ZESS_PA0182Table ZESS_PA0182 = new ZTHROMB004_1.ZESS_PA0182Table();
                ZTHROMB004_1.ZESS_PA0185Table ZESS_PA0185 = new ZTHROMB004_1.ZESS_PA0185Table();
                ZTHROMB004_1.ZESS_PA0364Table ZESS_PA0364 = new ZTHROMB004_1.ZESS_PA0364Table();

                ZTHROMB004_1.ZTHROMB004_1 bapi = new ZTHROMB004_1.ZTHROMB004_1();
                System.Net.NetworkCredential net = new System.Net.NetworkCredential();
                SAP.Connector.Destination Connector = new SAP.Connector.Destination();

                Connector.AppServerHost = SAP_AppServerHost;
                Connector.SystemNumber = 0;
                Connector.Client = Convert.ToInt16(SAP_Client);
                Connector.Username = SAP_Username;
                Connector.Password = SAP_Password;

                bapi.Connection = new SAP.Connector.SAPConnection(Connector);


                bapi.Zthromb004(Im_Chg_Date, Im_Infty, Im_Subty, out Ex_Return, ref Tab_Pa0002, ref Tab_Pa0006
                    , ref ZESS_PA0019, ref ZESS_PA0021, ref ZESS_PA0022, ref ZESS_PA0041, ref ZESS_PA0182
                    , ref ZESS_PA0185, ref ZESS_PA0364);

                string sql_log = @"insert into  BZ_TRANS_LOG (module,event) values (to_char(sysdate,'rrrrMMdd HHMMSS'),1)";
                msg = conn_ExecuteNonQuery(sql_log, false);

                if (ZESS_PA0019 == null)
                    return "";

                int icount = ZESS_PA0019.Count;

                sql_log = @"insert into  BZ_TRANS_LOG (module,event) values (to_char(sysdate,'rrrrMMdd HHMMSS'),2)";
                conn = new ws_conn.wsConnection();
                msg = conn_ExecuteNonQuery(sql_log, false);

                for (int i = 0; i < icount; i++)
                {
                    string Mndat = (ZESS_PA0019[i].Mndat ?? "").Trim();
                    string Termn = (ZESS_PA0019[i].Termn ?? "").Trim();
                    string Pernr = (ZESS_PA0019[i].Pernr ?? "").Trim();
                    string Subty = (ZESS_PA0019[i].Subty ?? "").Trim();
                    if (Subty == "LC" || Subty == "AE")
                    {

                    }
                    else
                    {
                        continue;
                    }

                    int result = 0;
                    string sql = "select count(1) as xcount from BZ_ZESS_PA0019 ";
                    sql += " where PERNR='" + Pernr + "' ";
                    sql += " and SUBTY = '" + Subty + "' ";
                    sql += " and MNDAT= '" + Mndat + "' ";
                    sql += " and TERMN= '" + Termn + "' ";
                    conn = new ws_conn.wsConnection();
                    DataTable dt = new DataTable();
                    conn_ExecuteData(ref dt, sql);
                    if (dt != null && dt.Rows.Count > 0) { if (dt.Rows[0]["xcount"].ToString() != "0") { result = 1; } }

                    if (result == 1)
                    {
                        sql = "update BZ_ZESS_PA0019 set  ";
                        sql += " MNDAT ='" + Mndat + "' ";
                        sql += " , TERMN='" + Termn + "' ";
                        sql += " where PERNR='" + Pernr + "' ";
                        sql += " and SUBTY = '" + Subty + "' ";
                        sql += " and MNDAT= '" + Mndat + "' ";
                        sql += " and TERMN= '" + Termn + "' ";
                    }
                    else
                    {
                        sql = "insert into BZ_ZESS_PA0019 (MNDAT, TERMN, PERNR, SUBTY) ";
                        sql += " values ('" + Mndat + "', '" + Termn + "', '" + Pernr + "', '" + Subty + "') ";
                    }
                    conn = new ws_conn.wsConnection();
                    msg = conn_ExecuteNonQuery(sql, false);
                    if (msg != "true")
                    {
                        msg += " error rows: " + i;
                        msg += " SAP_AppServerHost: " + SAP_AppServerHost;
                        msg += " sql: " + sql;
                        break;
                    }
                }

                return msg.ToString();
            }
            catch (Exception ex)
            {
                string sql_log = @"insert into  BZ_TRANS_LOG (module,event) values (to_char(sysdate,'rrrrMMdd HHMMSS'),3)";
                msg = conn_ExecuteNonQuery(sql_log, false);
                msg = ex.ToString();
            }

            return msg;
        }

    }
}