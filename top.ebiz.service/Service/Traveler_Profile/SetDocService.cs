using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using top.ebiz.service.Models.Traveler_Profile;
//using Oracle.ManagedDataAccess.Client;
//using System.Data.Entity;

//using System.Data.OracleClient;
//using Newtonsoft.Json;
using System.Text.Json;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;
using OfficeOpenXml;
//using System.IO;
//using System.DirectoryServices;
//using System.Data.OleDb;
//using OfficeOpenXml;
//using System.Web.Script.Serialization;

namespace top.ebiz.service.Service.Traveler_Profile 
{

    public class SetDocService
    {
        cls_connection_ebiz conn;
        string sqlstr = "";
        string sqlstr_all = "";
        string ret = "";
        DataTable dt;
        string SystemUser = System.Configuration.ConfigurationManager.AppSettings["SystemUser"].ToString();
        string SystemPass = System.Configuration.ConfigurationManager.AppSettings["SystemPass"].ToString();

        internal static string conn_ptai_ExecuteData(ref DataTable dtSelect, string sqlstr)
        {
            dtSelect = new DataTable();
            try
            {
                //adapter_data
                //ws_conn.wsConnection conn_ws = new ws_conn.wsConnection();
                //dtSelect = conn_ws.adapter_data_ptai(sqlstr);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        internal static string conn_ExecuteData(ref DataTable dtSelect, string sqlstr)
        {
            dtSelect = new DataTable();
            try
            {
                //adapter_data
                //ws_conn.wsConnection conn_ws = new ws_conn.wsConnection();
                //dtSelect = conn_ws.adapter_data(sqlstr);
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
                string ret = "";
                //ws_conn.wsConnection conn_ws = new ws_conn.wsConnection();
                //ebiz.webservice.Table.wsConnection conn_ws = new Table.wsConnection();
                //string ret = conn_ws.execute_data(sqlstr, type_check);
                if (ret == "") { ret = "true"; }
                if (ret.ToLower() == "true") { ret = "true"; }
                return ret;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        #region Function CPAI 
        //เอาไว้ใน project batch --> เดียวพี่เจตส่งให้
        //ดึงข้อมูลจาก select *  from CMDB.VW_FX_TYPE_M  --> EOSL.BZ_DATA_FX_TYPE_M 
        //public ExchangeRatesModel ExchangeRates()
        //{
        //    ExchangeRatesModel ex_rate = new ExchangeRatesModel();
        //    DataTable dt = new DataTable();
        //    cls_connection_cpai conn = new cls_connection_cpai();
        //    conn.OpenConnection();
        //    dt = conn.ExecuteAdapter(@" select t_fxb_cur as ex_cur, t_fxb_value1 as ex_value1
        //                                ,to_char(to_date(t_fxb_valdate,'yyyyMMdd') ,'dd Mon rrrr')   as ex_date
        //                                from CMDB.VW_FX_TYPE_M 
        //                                where  t_fxb_cur = 'USD' 
        //                                and t_fxb_valdate in (select  max(t_fxb_valdate) from CMDB.VW_FX_TYPE_M where  t_fxb_cur = 'USD' )").Tables[0];
        //    conn.CloseConnection(); 
        //    if (dt.Rows.Count > 0)
        //    {
        //        ex_rate = new ExchangeRatesModel();
        //        ex_rate.ex_value1 = dt.Rows[0]["ex_value1"].ToString();
        //        ex_rate.ex_cur = dt.Rows[0]["ex_cur"].ToString();
        //        ex_rate.ex_date = dt.Rows[0]["ex_date"].ToString();
        //    } 
        //    return ex_rate;
        //}
        #endregion Function CPAI


        #region Function  
        private DateTime? chkDate(string value)
        {
            DateTime? date = null;
            try
            {
                if (value == null)
                    return date;

                if (value.Length < 10)
                    return date;

                date = DateTime.ParseExact(value.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            }
            catch (Exception ex)
            {

            }
            return date;
        }

        private string retCheckValue(string value)
        {
            string ret = "N";
            try
            {
                if (value == "true")
                    ret = "Y";
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        private decimal? retDecimal(string value)
        {
            decimal? ret = null;
            try
            {
                ret = string.IsNullOrEmpty(value) ? ret : Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        private decimal toDecimal(string value)
        {
            decimal ret = 0;
            try
            {
                ret = string.IsNullOrEmpty(value) ? ret : Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        public DataTable CheckAllData(string tablename, string doc_id)
        {
            dt = new DataTable();
            sqlstr = "select * from " + tablename.ToLower() + " where 1=1 ";
            if (doc_id != "") { sqlstr += " and doc_id = '" + doc_id + "'"; }
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
            }
            return dt;
        }
        public int GetMaxID(string tablename)
        {
            dt = new DataTable();
            sqlstr = "select (nvl( max(to_number(id)),0)+1)as id from " + tablename.ToLower();
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        if (dt.Rows[0]["id"].ToString() != "") { return Convert.ToInt32(dt.Rows[0]["id"].ToString()); }
                    }
                    catch { }
                }
            }
            return 1;
        }
        public int GetMaxIDYear(string tablename)
        {
            dt = new DataTable();
            sqlstr = "select (nvl( max(to_number(id)),0)+1)as id from " + tablename.ToLower() + " where year = to_char(sysdate,'rrrr') ";
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        if (dt.Rows[0]["id"].ToString() != "") { return Convert.ToInt32(dt.Rows[0]["id"].ToString()); }
                    }
                    catch { }
                }
            }
            return 1;
        }
        public string sqlEmpRole(string token_login, ref string user_id, ref string user_role, ref Boolean user_admin, string doc_id)
        {
            user_id = ""; user_role = ""; user_admin = false;
            sqlstr = @" SELECT distinct a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code
                        FROM bz_login_token a left join vw_bz_users u on a.user_login = u.userid
                        left join bz_data_manage m on (m.pmsv_admin = 'true' or m.pmdv_admin = 'true') and m.emp_id = a.user_id
                        WHERE a.TOKEN_CODE = '" + token_login + "'  ";

            dt = new DataTable();
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow login_empid = dt.Rows[0];
                    user_id = login_empid["user_id"].ToString() ?? "";
                    user_role = login_empid["user_role"].ToString() ?? "";
                }
            }


            if (user_role == "1") { user_admin = true; }
            else
            {
                sqlstr = " select emp_id from bz_data_manage where ( pmsv_admin = 'true' ) and emp_id = '" + user_id + "'";
                DataTable login_empid = new DataTable();
                conn = new cls_connection_ebiz();
                if (SetDocService.conn_ExecuteData(ref login_empid, sqlstr) == "")
                {
                    if (login_empid != null && login_empid.Rows.Count > 0)
                    {
                        user_admin = true;
                    }
                }
            }

            if (user_admin == false)
            {
                try
                {
                    if (doc_id.IndexOf("T") > -1)
                    {
                        sqlstr = " select emp_id from bz_data_manage where ( pmdv_admin = 'true' ) and emp_id = '" + user_id + "'";
                        DataTable login_empid = new DataTable();
                        conn = new cls_connection_ebiz();
                        if (SetDocService.conn_ExecuteData(ref login_empid, sqlstr) == "")
                        {
                            if (login_empid != null && login_empid.Rows.Count > 0)
                            {
                                user_admin = true;
                            }
                        }
                    }
                }
                catch { }
            }



            return "";
        }

        public string sqlEmpUserID(string token_login)
        {
            dt = new DataTable();
            sqlstr = @" SELECT a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code
                        FROM bz_login_token a left join vw_bz_users u on a.user_login = u.userid
                        WHERE a.TOKEN_CODE = '" + token_login + "'  ";

            dt = new DataTable();
            conn = new cls_connection_ebiz();
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow login_empid = dt.Rows[0];
                    return login_empid["user_id"].ToString() ?? "";
                }
            }
            return "";
        }
        public string sqlEmpUserName(string token_login)
        {
            dt = new DataTable();
            sqlstr = @" SELECT a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code
                        FROM bz_login_token a left join vw_bz_users u on a.user_login = u.userid
                        WHERE a.TOKEN_CODE = '" + token_login + "'  ";

            dt = new DataTable();
            conn = new cls_connection_ebiz();
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow login_empid = dt.Rows[0];
                    return login_empid["user_name"].ToString() ?? "";
                }
            }
            return "";
        }
        public string sqlEmpUserDispayName(string token_login)
        {
            dt = new DataTable();
            sqlstr = @" SELECT a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code
                        ,case when u.usertype = 2 then u.enfirstname else nvl(u.entitle, '')|| ' ' || u.enfirstname || ' ' || u.enlastname  end userdisplay
                        FROM bz_login_token a left join vw_bz_users u on a.user_login = u.userid
                        WHERE a.TOKEN_CODE = '" + token_login + "'  ";

            dt = new DataTable();
            conn = new cls_connection_ebiz();
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow login_empid = dt.Rows[0];
                    return login_empid["userdisplay"].ToString() ?? "";
                }
            }
            return "";
        }
        public string sqlEmpUserMail(string token_login)
        {
            dt = new DataTable();
            sqlstr = @" SELECT a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code, u.email
                        ,case when u.usertype = 2 then u.enfirstname else nvl(u.entitle, '')|| ' ' || u.enfirstname || ' ' || u.enlastname  end userdisplay
                        FROM bz_login_token a left join vw_bz_users u on a.user_login = u.userid
                        WHERE a.TOKEN_CODE = '" + token_login + "'  ";

            dt = new DataTable();
            conn = new cls_connection_ebiz();
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow login_empid = dt.Rows[0];
                    return login_empid["email"].ToString() ?? "";
                }
            }
            return "";
        }

        #endregion Function  

        #region Function in Doc
        public List<Users> SetGroupSystemAdmin()
        {
            string group_key_name = "Group System Admin";
            DataTable dtadmin = new DataTable();
            List<Users> lstADUsers = new List<Users>();
            try
            {
                if (true)
                {
                    DataTable dt = new DataTable();
                    sqlstr = @"select key_value as name  from  bz_config_data where trim(lower(key_name)) =trim(lower('" + group_key_name + "')) ";
                    if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                    {
                        if (dt.Rows.Count > 0)
                        {
                            string UserName = SystemUser.ToUpper();
                            string Password = SystemPass;

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i]["name"].ToString() == "") { continue; }
                                string memberOf = "CN=" + dt.Rows[i]["name"].ToString() + ",OU=Distribution Group,OU=E-Mail Resources,OU=Resources Management,DC=thaioil,DC=localnet";

                                String domain = "ThaioilNT";

                                String str = String.Format("LDAP://{0}", domain);
                                String str2 = String.Format(("{0}\\" + UserName.Trim()), domain);
                                String pass = Password;
                                //DirectoryEntry Entry = new DirectoryEntry(str, str2, pass);
                                //DirectorySearcher search = new DirectorySearcher(Entry);
                                //search.Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=" + memberOf + "))";
                                //search.PropertiesToLoad.Add("samaccountname");
                                //search.PropertiesToLoad.Add("mail");
                                //search.PropertiesToLoad.Add("usergroup");
                                //search.PropertiesToLoad.Add("displayname");//first name
                                //search.PropertiesToLoad.Add("memberOf");
                                //SearchResult result;
                                //SearchResultCollection resultCol = search.FindAll();

                                //SearchResult xxx = search.FindOne();
                                //if (resultCol != null)
                                //{
                                //    for (int counter = 0; counter < resultCol.Count; counter++)
                                //    {
                                //        string UserNameEmailString = string.Empty;
                                //        result = resultCol[counter];
                                //        if (result.Properties.Contains("samaccountname") &&
                                //                 result.Properties.Contains("mail") &&
                                //            result.Properties.Contains("displayname"))
                                //        {

                                //            Users objSurveyUsers = new Users();
                                //            objSurveyUsers.Email = (String)result.Properties["mail"][0];
                                //            objSurveyUsers.UserName = (String)result.Properties["samaccountname"][0];
                                //            objSurveyUsers.DisplayName = (String)result.Properties["displayname"][0];
                                //            objSurveyUsers.MemberOf = (String)result.Properties["memberOf"][0];
                                //            lstADUsers.Add(objSurveyUsers);
                                //        }

                                //    }
                                //}


                            }
                        }
                    }

                    if (true)
                    {
                        //update role_id = 1 
                        if (lstADUsers.Count > 0)
                        {
                            for (int i = 0; i < lstADUsers.Count; i++)
                            {
                                sqlstr = "select * from  bz_users where lower(userid) =lower('" + lstADUsers[i].UserName + "') ";
                                dt = new DataTable();
                                if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                                {
                                    if (dt.Rows.Count == 0)
                                    {
                                        //เพิ่ม/update ข้อมูลในตาราง BZ_USERS  
                                        sqlstr = @" call bz_sp_add_user_z ( ";
                                        sqlstr += " 'system'";
                                        sqlstr += ",'" + lstADUsers[i].UserName.ToString().ToUpper() + "'";
                                        sqlstr += ",'" + lstADUsers[i].DisplayName.ToString() + "'";
                                        sqlstr += ",'" + lstADUsers[i].Email.ToString() + "'";
                                        sqlstr += ",'insert'";
                                        sqlstr += ")";
                                        //Stored Procedure ถ้า call จะ update ใน db เลย
                                        ret = conn_ExecuteNonQuery(sqlstr, false);

                                    }

                                    ////เนื่องจากมีหน้า maintain ข้อมูล admin ไม่ต้อง set ให้ auto
                                    //sqlstr = " update bz_users set role_id =1 where lower(userid) =lower('" + lstADUsers[i].UserName + "') ";
                                    //sqlstr_all += sqlstr + "||";
                                    //ret = conn_ExecuteNonQuery(sqlstr, false);
                                    //if (ret.ToLower() != "true") { goto Next_line_1; }

                                    lstADUsers[i].Remark = ret + " --> sqlstr : " + sqlstr;
                                }
                            }
                        Next_line_1:;
                            if (ret.ToLower() == "true")
                            {
                                //sqlstr = " update bz_users set role_id = null where  lower(userid) not in ('zattaphonso','zkul-uwat')";
                                //sqlstr_all += sqlstr + "||" + sqlstr_all;
                                ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                            }
                        }
                    }

                }
            }
            catch { }
            return lstADUsers;
        }

        public ImgList SetTravelerHistoryImg(ImgList value)
        {
            var data = value;
            var token_login = value.modified_by;//ส่งมาเป็น emp ??? เดียวค่อยเครียร์ว่าจะใช้อะไรกันแน่
            ret = "";

            //logService.logModel mLog = new logService.logModel();
            //
            //mLog.module = "UploadFile:SetTravelerHistoryImg-Start";
            //mLog.tevent = "1";
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(data);
            //logService.insertLog(mLog);

            try
            {
                #region set data 

                conn = new cls_connection_ebiz();
                DataTable dtcheck = new DataTable();
                sqlstr = @"select count(1) as xcount from BZ_USER_PEOFILE where EMPLOYEEID = " + conn.ChkSqlStr(data.emp_id, 300);
                conn_ExecuteData(ref dtcheck, sqlstr);

                Boolean bcheckInsert = false;
                if (dtcheck.Rows.Count > 0)
                {
                    if (dtcheck.Rows[0]["xcount"].ToString() == "0") { bcheckInsert = true; }
                }

                if (bcheckInsert == true)
                {
                    int imaxid = 1;
                    dt = new DataTable();
                    sqlstr = "select (nvl( max(to_number(id)),0)+1)as id from BZ_USER_PEOFILE";
                    if (conn_ExecuteData(ref dt, sqlstr) == "")
                    {
                        if (dt.Rows.Count > 0)
                        {
                            try
                            {
                                if (dt.Rows[0]["id"].ToString() != "") { imaxid = Convert.ToInt32(dt.Rows[0]["id"].ToString()); }
                            }
                            catch { }
                        }
                    }
                    sqlstr = @" insert into BZ_USER_PEOFILE (ID,DOC_ID,EMPLOYEEID
                                    ,IMGPATH,IMGPROFILENAME,CREATE_BY,CREATE_DATE,UPDATE_BY,UPDATE_DATE,TOKEN_UPDATE)" +
                                   " values (" +
                                   " " + imaxid +
                                   " ," + conn.ChkSqlStr("personal", 300) +
                                   " ," + conn.ChkSqlStr(data.emp_id, 300) +
                                   " ," + conn.ChkSqlStr(data.path, 300) +
                                   " ," + conn.ChkSqlStr(data.filename, 300) +
                                   " ," + conn.ChkSqlStr("system", 300) +
                                   " ,sysdate,null,null" +
                                   " ," + conn.ChkSqlStr(token_login, 300) +
                                   " ) ";
                }
                else
                {
                    sqlstr = " update BZ_USER_PEOFILE set " +
                               " IMGPATH = " + conn.ChkSqlStr(data.path, 300) +
                               " ,IMGPROFILENAME = " + conn.ChkSqlStr(data.filename, 300) +
                               " ,UPDATE_BY =" + conn.ChkSqlStr(token_login, 300) +
                               " ,UPDATE_DATE = sysdate" +
                               " where EMPLOYEEID = " + conn.ChkSqlStr(data.emp_id, 300);
                }
                ret = conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";

                if (ret.ToLower() != "true") { goto Next_line_1; }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }

                #endregion set data

            }
            catch (Exception ex) { ret = "error try "; }

            data.remark = ret;

            //mLog = new logService.logModel();
            //js = new JavaScriptSerializer();
            //mLog.module = "UploadFile:Img-End";
            //mLog.tevent = ret;
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(data);
            //logService.insertLog(mLog);


            return data;
        }
        public ImgList SetImgByPage(ImgList value, string action_type)
        {
            var data = value;
            var token_login = value.modified_by;//ส่งมาเป็น emp ??? เดียวค่อยเครียร์ว่าจะใช้อะไรกันแน่

            #region set data 
            int imaxid = GetMaxID("BZ_DOC_IMG");

            conn = new cls_connection_ebiz();

            if (action_type == "insert")
            {
                //กรณีที่มีข้อมูลเก่า id เดียวกันให้ ลบก่อน
                sqlstr = " update from BZ_DOC_IMG set " +
                          " STATUS = 0" +
                          " where EMP_ID = " + conn.ChkSqlStr(data.emp_id, 300) +
                          " and DOC_ID =" + conn.ChkSqlStr(data.doc_id, 300) +
                          " and ID =" + conn.ChkSqlStr(data.id, 300);

                ret = conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";
                if (ret.ToLower() != "true") { goto Next_line_1; }

                //กรณีที่มีข้อมูลเก่า id เดียวกันให้ ลบก่อน
                var id = "";
                if (data.id.ToString() == "") { id = imaxid.ToString(); imaxid++; }
                sqlstr = " insert into BZ_DOC_IMG (ID,DOC_ID,EMP_ID,PATH,FILE_NAME,PAGE_NAME,ACTION_NAME,STATUS,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) " +
                      " values ( " +
                      " " + conn.ChkSqlStr(id, 300) +
                      " , " + conn.ChkSqlStr(data.doc_id, 300) +
                      " , " + conn.ChkSqlStr(data.emp_id, 300) +
                      " , " + conn.ChkSqlStr(data.path, 300) +
                      " , " + conn.ChkSqlStr(data.filename, 300) +
                      " , " + conn.ChkSqlStr(data.filename, 300) +
                      " , " + conn.ChkSqlStr(data.pagename, 300) +
                      " , " + conn.ChkSqlStr(data.actionname, 300) +
                      " , " + conn.ChkSqlStr(data.modified_by, 300) +
                      " , 1" +
                      " , sysdate,null" +
                      " )";

                ret = conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";
                if (ret.ToLower() != "true") { goto Next_line_1; }
            }
            else if (action_type == "delete")
            {
                sqlstr = " update from BZ_DOC_IMG set " +
                           " STATUS = 0" +
                           " where EMP_ID = " + conn.ChkSqlStr(data.emp_id, 300) +
                           " and DOC_ID =" + conn.ChkSqlStr(data.doc_id, 300) +
                           " and ID =" + conn.ChkSqlStr(data.id, 300);


                ret = conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";

                if (ret.ToLower() != "true") { goto Next_line_1; }
            }

        Next_line_1:;

            if (ret.ToLower() == "true")
            {
                ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
            }

            #endregion set data


            return data;
        }
        public string CopyTravelerHistory(DataTable dtlist, string doc_type, string doc_id, string token_login, string emp_id_active)
        {
            #region set data 

            if (dtlist.Rows.Count > 0)
            {
                int imaxid = 1;
                dt = new DataTable();
                sqlstr = "select (nvl( max(to_number(id)),0)+1)as id from BZ_USER_PEOFILE";
                if (conn_ExecuteData(ref dt, sqlstr) == "")
                {
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            if (dt.Rows[0]["id"].ToString() != "") { imaxid = Convert.ToInt32(dt.Rows[0]["id"].ToString()); }
                        }
                        catch { }
                    }
                }

                conn = new cls_connection_ebiz();

                for (int i = 0; i < dtlist.Rows.Count; i++)
                {
                    var action_type = "insert";
                    if (action_type == "insert")
                    {
                        sqlstr = @" insert into BZ_USER_PEOFILE (ID,DOC_ID,EMPLOYEEID,USERID,MOBILE,TELEPHONE
                                    ,IMGPATH,IMGPROFILENAME,CREATE_BY,CREATE_DATE,UPDATE_BY,UPDATE_DATE,TOKEN_UPDATE)" +
                                 " values (" +
                                 " " + imaxid +
                                 " ," + conn.ChkSqlStr(doc_id, 300) +
                                 " ," + conn.ChkSqlStr(dtlist.Rows[i]["EMPLOYEEID"], 300) +
                                 " ," + conn.ChkSqlStr(dtlist.Rows[i]["USERID"], 300) +
                                 " ," + conn.ChkSqlStr(dtlist.Rows[i]["MOBILE"], 300) +
                                 " ," + conn.ChkSqlStr(dtlist.Rows[i]["TELEPHONE"], 300) +
                                 " ," + conn.ChkSqlStr(dtlist.Rows[i]["IMGPATH"], 300) +
                                 " ," + conn.ChkSqlStr(dtlist.Rows[i]["IMGPROFILENAME"], 300) +
                                 " ," + conn.ChkSqlStr(emp_id_active, 300) +
                                 " ,sysdate,null,null" +
                                 " ," + conn.ChkSqlStr(token_login, 300) +
                                 " );";
                        imaxid++;
                    }

                    ret = conn_ExecuteNonQuery(sqlstr, true);
                    sqlstr_all += sqlstr + "||";

                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }

            }
            #endregion set data

            return ret.ToLower();
        }


        public UploadFileModel uploadfile_data(UploadFileModel value)
        {
            var data = new UploadFileModel();
            DataTable dtdef = new DataTable();
            //HttpResponse response = HttpContext.Current.Response;
            //HttpFileCollection files = HttpContext.Current.Request.Files;

            string _Folder = "/Image/" + value.file_doc + "/" + value.file_page + "/" + value.file_emp + "/";
            //string _Path = System.Web.HttpContext.Current.Server.MapPath("~" + _Folder);
            string _FullPathName = "";//System.Web.HttpContext.Current.Server.MapPath("~/AttachedFile/Plan/" + file.FileName);
            string _FileName = "";
            string ret = "";

            #region Determine whether the directory exists.
            string msg_error = "";
            try
            {
                //if (files == null)
                //{
                //    msg_error = "Select a file to upload.";
                //    goto next_line_1;
                //}
                //if (files.Count == 0)
                //{
                //    msg_error = "Select a file to upload.";
                //    goto next_line_1;
                //}
                //DirectoryInfo di = Directory.CreateDirectory(_Path);
                ////ลบจริงตอน save
                //if (Directory.Exists(_Path))
                //{
                //    //delete all files and folders in a directory 
                //    //foreach (FileInfo file in di.GetFiles())
                //    //{
                //    //    file.Delete();
                //    //} 
                //}

                //for (int i = 0; i < files.Count; i++)
                //{
                //    HttpPostedFile file = files[i];
                //    _FileName = file.FileName;
                //    _FullPathName = _Path + _FileName;//System.Web.HttpContext.Current.Server.MapPath("~/AttachedFile/Plan/" + file.FileName);
                //    file.SaveAs(_FullPathName);
                //    ret = "true";
                //}

            }
            catch (Exception ex) { msg_error = ex.Message.ToString(); }

        #endregion Determine whether the directory exists.
        next_line_1:;

            data.after_trip.opt1 = (ret ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret ?? "") == "true" ? "Upload file succesed." : "Upload file failed.";
            data.after_trip.opt2.remark = (ret ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "";
            data.after_trip.opt3.remark = "";

            return data;
        }
        public string openfile_excel_check(string fullpath, string token_login, string emp_user_active)
        {
            string ret = "true";
            try
            {
                FileInfo fileInfo = new FileInfo(fullpath);
                ExcelPackage package = new ExcelPackage(fileInfo);
            }
            catch (Exception ex) { ret = ex.Message.ToString(); }

            return ret;
        }
        public string readfile_excel_check(string fullpath, string token_login, string emp_user_active)
        {
            string ret = "true";
            try
            {
                ImportDataKH_Code(fullpath, token_login);
            }
            catch (Exception ex) { ret = ex.Message.ToString(); }

            return ret;
        }
        public UploadFileModel uploadfile_data_form()
        {
            var data = new UploadFileModel();
            DataTable dtdef = new DataTable();
            //HttpResponse response = HttpContext.Current.Response;
            //HttpFileCollection files = HttpContext.Current.Request.Files;

            string msg_error = "";
            string msg_rows = "";
            string ret = "";
            string _FullPathName = "";//System.Web.HttpContext.Current.Server.MapPath("~/AttachedFile/Plan/" + file.FileName);
            string _FileName = "";
            string _Server_path = "http://TBKC-DAPPS-05.thaioil.localnet/ebiz_ws";
            _Server_path = System.Configuration.ConfigurationManager.AppSettings["ServerPath_Service"].ToString();
            //http://TBKC-DAPPS-05.thaioil.localnet/ebiz_ws/
            //Image/OB20110006/passport/00000910/foreignpassportrus.jpg

            try
            {
                //var httpRequest = HttpContext.Current.Request;
                var file_doc = "";
                var file_page = "";
                var file_emp = "";
                var file_typename = "";
                var file_token_login = "";

                //file_typename = action_change_imgname --> มีเฉพาะหน้า portal
                //เพิ่มใช้ file_typename = 'auto_generate' กรณีที่เป็น auto generate
                try
                {
                    msg_rows = "error data : file_typename ";
                    //file_typename = httpRequest.Form["file_typename"].ToString();
                }
                catch { }
                try
                {
                    msg_rows = "error data : file_token_login ";
                    //file_token_login = httpRequest.Form["file_token_login"].ToString();
                }
                catch { }

                msg_rows = "error data : file_doc ";
                //file_doc = httpRequest.Form["file_doc"].ToString();
                msg_rows = "error data : file_page ";
                try
                {
                    //file_page = httpRequest.Form["file_page"].ToString().Trim();

                    if (file_page == "kh code") { file_page = "khcode"; }
                }
                catch { }
                msg_rows = "error data : file_emp ";
                //file_emp = httpRequest.Form["file_emp"].ToString();
                msg_rows = "";

                string _foler_name = "Image";
                string _Folder = "/" + _foler_name + "/" + file_doc + "/" + file_page + "/" + file_emp + "/";
                if (file_page == "isos" || file_page == "portal")
                {
                    _foler_name = "DocumentFile";
                    _Folder = "/" + _foler_name + "/" + file_page + "/";
                    if (file_typename != "")
                    {
                        _Folder += file_typename + "/";
                    }
                }
                else if (file_page == "mtvisacountries")
                {
                    file_doc = "master visa docountries";
                    _Folder = "/" + _foler_name + "/" + file_doc + "/" + file_page + "/";
                }
                else if (file_page == "allowance")
                {
                    if (file_typename == "auto_generate")
                    {
                        _foler_name = "ExportFile";
                        _Folder = "/" + _foler_name + "/" + file_doc + "/" + file_page + "/" + file_emp + "/";
                    }
                }
                else if (file_page == "passport")
                {
                    file_doc = "personal";
                    _Folder = "/" + _foler_name + "/" + file_doc + "/" + file_page + "/" + file_emp + "/";
                }
                else if (file_page == "khcode")
                {
                    _foler_name = "DocumentFile";
                    _Folder = "/" + _foler_name + "/" + file_page + "/";
                }

                //string _Path = System.Web.HttpContext.Current.Server.MapPath("~" + _Folder);

                #region Determine whether the directory exists.
                try
                {
                    //if (files == null)
                    //{
                    //    msg_error = "Select a file to upload.";
                    //    goto next_line_1;
                    //}
                    //if (files.Count == 0)
                    //{
                    //    msg_error = "Select a file to upload.";
                    //    goto next_line_1;
                    //}
                    //DirectoryInfo di = Directory.CreateDirectory(_Path);
                    ////ลบจริงตอน save
                    //if (Directory.Exists(_Path))
                    //{
                    //    //delete all files and folders in a directory 
                    //    //foreach (FileInfo file in di.GetFiles())
                    //    //{
                    //    //    file.Delete();
                    //    //} 
                    //}

                    //for (int i = 0; i < files.Count; i++)
                    //{
                    //    HttpPostedFile file = files[i];
                    //    _FileName = file.FileName;

                    //    _FileName = _FileName.Replace(" ", "_").Replace("(", "").Replace(")", "");

                    //    _FullPathName = _Path + _FileName;//System.Web.HttpContext.Current.Server.MapPath("~/AttachedFile/Plan/" + file.FileName);
                    //    file.SaveAs(_FullPathName);
                    //    ret = "true";
                    //}

                }
                catch (Exception ex) { msg_error = ex.Message.ToString(); }

            #endregion Determine whether the directory exists.

            next_line_1:;

                if ((ret ?? "") == "true")
                {
                    string modified_by = "";
                    string modified_date = "";
                    try
                    {
                        if (file_token_login != "")
                        {
                            SetDocService sw = new SetDocService();
                            modified_by = sw.sqlEmpUserDispayName(file_token_login);
                        }

                    }
                    catch { }
                    try
                    {
                        modified_date = DateTime.Now.ToString("dd MMM yyyy");
                    }
                    catch { }

                    data.img_list.doc_id = file_doc;
                    data.img_list.emp_id = file_emp;
                    data.img_list.id = "1";

                    data.img_list.path = _Server_path + _Folder;
                    data.img_list.filename = _FileName;

                    data.img_list.fullname = _Server_path + _Folder + _FileName;

                    data.img_list.pagename = file_page;
                    data.img_list.action_type = "insert";
                    data.img_list.action_change = "true";

                    data.img_list.modified_by = modified_by;
                    data.img_list.modified_date = modified_date;

                    data.img_list.remark = file_token_login;
                    if (file_page == "travelerhistory")
                    {
                        //logService.logModel mLog = new logService.logModel();
                        //
                        //mLog.module = "UploadFile:file_page";
                        //mLog.tevent = "";
                        //mLog.ref_id = 0;
                        //mLog.data_log = JsonSerializer.Serialize(data);
                        //logService.insertLog(mLog);

                        SetTravelerHistoryImg(data.img_list);
                    }
                    else if (file_page == "khcode")
                    {
                        ret = ImportDataKH_Code(_FullPathName, file_token_login);
                        if (ret.ToLower() != "true") { msg_error = ret; }
                    }
                }
            }
            catch (Exception ex_msg) { msg_error = ex_msg.Message.ToString() + " ---- " + msg_rows; }


            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Upload file succesed." : "Upload file failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "SaveAs FullPathName";
            data.after_trip.opt3.remark = _FullPathName;

            return data;
        }
        public string ImportDataKH_Code(string _FullPathName, string token_login)
        {
            try
            {
                string emp_name = sqlEmpUserName(token_login);
                sw_WriteLine("ImEx0", "start import excel " + _FullPathName);
                ret = import_excel_kh_code(_FullPathName, token_login, emp_name);
                sw_WriteLine("ImEx1", "end import excel " + sqlstr);
            }
            catch (Exception ex) { ret += ex.Message.ToString(); }
            if (ret.ToLower() != "true") { return ret; }

            // Open the stream and read it back.
            using (FileStream file = File.Open(_FullPathName, FileMode.Open, FileAccess.Read))
            {
                sw_WriteLine("Im1", "Open the stream and read it back" + token_login + " file " + _FullPathName);

                string emp_name = sqlEmpUserName(token_login);
                string file_name = "";
                string file_size = "";
                string path = "";
                // Get file size  
                long size = file.Length;
                file_name = file.Name.ToString();
                file_size = (file.Length).ToString();

                FileInfo fileInfo = new FileInfo(_FullPathName);
                string directoryFullPath = fileInfo.DirectoryName; // contains "C:\MyDirectory"
                path = directoryFullPath + @"\";
                file_name = fileInfo.Name.ToString();

                sqlstr_all = "";
                //นำข้อมูล update in table BZ_DATA_KH_CODE 
                sqlstr = "delete from BZ_FILE_DATA  where page_name ='khcode'";
                ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";

                sqlstr = "insert into BZ_FILE_DATA (id, page_name, file_name, file_size, path, create_by, create_date, token_update)";
                sqlstr += " values ";
                sqlstr += " ( ";
                sqlstr += " 0";
                sqlstr += " , " + conn.ChkSqlStr("khcode", 4000);
                sqlstr += " , " + conn.ChkSqlStr(file_name, 4000);
                sqlstr += " , " + conn.ChkSqlStr(file_size, 300);
                sqlstr += " , " + conn.ChkSqlStr(path, 300);
                sqlstr += " , " + conn.ChkSqlStr(emp_name, 300);
                sqlstr += " , sysdate";
                sqlstr += " , " + conn.ChkSqlStr(token_login, 300);
                sqlstr += " ) ";
                ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";
                if (ret == "true")
                {
                    try
                    {
                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                    }
                    catch (Exception ex) { ret += ex.Message.ToString(); }
                }
                sw_WriteLine("Im3", ret + " sql " + sqlstr_all);
            }

            return ret;
        }
        public string import_excel_kh_code(string fullpath, string token_login, string emp_user_active)
        {
            DataTable dtcol = new DataTable();
            dtcol.Columns.Add("emp_id");
            dtcol.Columns.Add("oversea_code");
            dtcol.Columns.Add("local_code");
            dtcol.AcceptChanges();

            var imsg_rows = 1;
            string ret = "";
            try
            {
                sw_WriteLine("name11", " path to your excel file ");

                //// path to your excel file 
                //fullpath = @"D:\Ebiz2\EBiz_Webservice\DocumentFile\khcode\KH_QR_CODE.xlsx";
                imsg_rows = 2;
                ExcelPackage ExcelPkg = new ExcelPackage();
                imsg_rows = 3;
                ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
                imsg_rows = 4;

                FileInfo fileInfo = new FileInfo(fullpath);
                imsg_rows = 5;

                ExcelPackage package = new ExcelPackage(fileInfo);
                imsg_rows = 6;

                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                imsg_rows = 7;
                // get number of rows and columns in the sheet
                int rows = worksheet.Dimension.Rows; // 20
                int columns = worksheet.Dimension.Columns; // 7
                int irows = 0;
                imsg_rows = 8;

                sw_WriteLine("name12", "loop through the worksheet rows " + rows);
                string emp_id = "";
                string oversea_code = "";
                string local_code = "";
                // loop through the worksheet rows and columns
                for (int i = 2; i < rows; i++)
                {
                    //column 1: emp --> 913 ต้องเพิ่ม 000000000 ให้ครบสิบหลัก
                    //column 9: Overseas --> OA2
                    //column 11: Local Allo --> LA2 
                    try
                    {
                        emp_id = worksheet.Cells[i, 1].Text.ToString();
                        oversea_code = worksheet.Cells[i, 9].Text.ToString();
                        local_code = worksheet.Cells[i, 11].Text.ToString();
                        if (emp_id.ToString() == "") { break; }

                        dtcol.Rows.Add(dtcol.NewRow());
                        dtcol.AcceptChanges();
                        dtcol.Rows[irows]["emp_id"] = emp_id;
                        dtcol.Rows[irows]["oversea_code"] = oversea_code;
                        dtcol.Rows[irows]["local_code"] = local_code;
                        dtcol.AcceptChanges();

                        irows++;

                        //sw_WriteLine("namex" + irows, emp_id.ToString() + " ,oversea_code :" + oversea_code + " ,local_code:" + local_code); 

                    }
                    catch (Exception ex1)
                    {
                        sw_WriteLine("name x" + irows, "rows error: " + irows);
                        //ret = "rows error: " + irows + " fullpath: " + fullpath + " --> open excel " + ex1.Message.ToString();
                        //sw_WriteLine("namex" + irows, "rows error: " + irows + " emp:"+emp_id.ToString() + " ,oversea_code :" + oversea_code + " ,local_code:" + local_code);
                        break;
                    }


                }


            }
            catch (Exception ex)
            {
                ret = "rows error: " + imsg_rows + " fullpath: " + fullpath + " --> open excel " + ex.Message.ToString();
                return ret;
            }

            Boolean bCheckStepUpdateData = false;
            try
            {
                SetDocService wss = new SetDocService();
                Boolean bNewData = false;
                int imaxid = wss.GetMaxID("BZ_DATA_KH_CODE");
                var sqlstr_all = "";
                var bCheckQuery = false;
                for (int i = 0; i < dtcol.Rows.Count; i++)
                {
                    string user_id = "";
                    string emp_id = dtcol.Rows[i]["emp_id"].ToString();
                    string oversea_code = dtcol.Rows[i]["oversea_code"].ToString();
                    string local_code = dtcol.Rows[i]["local_code"].ToString();
                    if (emp_id.ToString() == "") { continue; }

                    if (bNewData == false)
                    {
                        #region copy ข้อมูลเดิมไว้กอน
                        sqlstr = "delete from BZ_DATA_KH_CODE_BEFOR ";
                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, bCheckQuery);
                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        sqlstr = "insert into BZ_DATA_KH_CODE_BEFOR select * from  BZ_DATA_KH_CODE ";
                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, bCheckQuery);
                        if (ret.ToLower() != "true") { goto Next_line_1; }
                        #endregion copy ข้อมูลเดิมไว้กอน

                        bNewData = true;
                        sqlstr = "delete from BZ_DATA_KH_CODE ";
                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, bCheckQuery);
                        sqlstr_all += sqlstr + "||";
                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        bCheckStepUpdateData = true;
                    }

                    if (emp_id.Length < 8)
                    {
                        emp_id = ("00000000" + emp_id).Substring(emp_id.Length, 8);
                    }


                    sqlstr = @" insert into BZ_DATA_KH_CODE
                                    (ID,EMP_ID,USER_ID,OVERSEA_CODE,LOCAL_CODE,STATUS
                                    ,DATA_FOR_SAP
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                    conn = new cls_connection_ebiz();
                    sqlstr += @" " + imaxid;
                    sqlstr += @" ," + conn.ChkSqlStr(emp_id, 300);

                    sqlstr += @" ," + conn.ChkSqlStr(user_id, 300);
                    sqlstr += @" ," + conn.ChkSqlStr(oversea_code, 300);
                    sqlstr += @" ," + conn.ChkSqlStr(local_code, 300);
                    sqlstr += @" ," + conn.ChkSqlStr("1", 300);

                    sqlstr += @" ," + conn.ChkSqlStr("0", 300); //sap = 1, ebiz = 0

                    sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                    sqlstr += @" ,sysdate";
                    sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                    sqlstr += @" )";

                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr, bCheckQuery);
                    sqlstr_all += sqlstr + "||";

                    if (ret.ToLower() != "true") { goto Next_line_1; }
                    imaxid++;
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    //sqlstr = "update BZ_DATA_KH_CODE a set emp_id =  substr('00000000' || a.emp_id,length('00000000' || a.emp_id)-7) ";
                    //ret = SetDocService.conn_ExecuteNonQuery(sqlstr, bCheckQuery);
                    //sqlstr_all += sqlstr + "||"; 
                    //ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, bCheckQuery);
                }
                else
                {

                    #region copy ข้อมูลเดิมกลับ 
                    if (bCheckStepUpdateData == true)
                    {
                        sqlstr = "delete from BZ_DATA_KH_CODE ";
                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, bCheckQuery);
                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        sqlstr = "insert into BZ_DATA_KH_CODE select * from  BZ_DATA_KH_CODE_BEFOR ";
                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, bCheckQuery);
                        if (ret.ToLower() != "true") { goto Next_line_1; }
                    }
                    #endregion copy copy ข้อมูลเดิมกลับ

                }
            }
            catch (Exception ex) { ret = "import excel to table " + ex.Message.ToString() + " จำนวนข้อมูล : " + dtcol.Rows.Count; }

            //sw_WriteLine("e1", ret);

            return ret;

        }

        private void sw_WriteLine(string name_x, string msg_ref)
        {
            string ServerFolder = System.Configuration.ConfigurationManager.AppSettings["ServerFolder"].ToString();
            string pathw = ServerFolder + @"\EBiz_Webservice\DocumentFile\khcode\MyTest" + name_x + ".txt";
            if (!File.Exists(pathw))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(pathw))
                {
                    sw.WriteLine(msg_ref);

                }
            }
        }

        public string SetImgList(List<ImgList> value, int imaxidImg, string emp_user_active, string token_login
            , ref cls_connection_ebiz conn, ref string sqlstr_all)
        {
            List<ImgList> dtlist = value;
            for (int i = 0; i < dtlist.Count; i++)
            {
                ret = "true"; sqlstr = "";
                var action_type = dtlist[i].action_type.ToString();
                if (action_type == "") { continue; }
                else if (action_type != "delete")
                {
                    var action_change = dtlist[i].action_change + "";
                    if (action_change.ToLower() != "true") { continue; }
                }

                if (action_type == "insert")
                {
                    string doc_id_def = dtlist[i].doc_id;
                    try
                    {
                        if (dtlist[i].actionname.ToString().ToLower() == "visa_page")
                        {
                            doc_id_def = "personal";
                        }
                    }
                    catch { dtlist[i].actionname = ""; }
                    sqlstr = " insert into BZ_DOC_IMG (ID,ID_LEVEL_1,ID_LEVEL_2,DOC_ID,EMP_ID,PATH,FILE_NAME,PAGE_NAME,ACTION_NAME,STATUS,ACTIVE_TYPE,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) " +
                      " values ( " +
                      " " + conn.ChkSqlStr(imaxidImg, 300) +
                      " , " + conn.ChkSqlStr(dtlist[i].id_level_1, 300) +
                      " , " + conn.ChkSqlStr(dtlist[i].id_level_2, 300) +
                      " , " + conn.ChkSqlStr(doc_id_def, 300) +
                      " , " + conn.ChkSqlStr(dtlist[i].emp_id, 300) +
                      " , " + conn.ChkSqlStr(dtlist[i].path, 300) +
                      " , " + conn.ChkSqlStr(dtlist[i].filename, 300) +
                      " , " + conn.ChkSqlStr(dtlist[i].pagename, 300) +
                      " , " + conn.ChkSqlStr(dtlist[i].actionname, 300) +
                      " , 1" +
                      " , " + conn.ChkSqlStr(dtlist[i].active_type, 300) +
                      " , " + conn.ChkSqlStr(emp_user_active, 300) +
                      " , sysdate" +
                      " , " + conn.ChkSqlStr(token_login, 300) +
                      " )";
                    imaxidImg++;
                }
                else
                {

                    var img_status = 0;//delete
                    if (action_type == "update")
                    {
                        img_status = 1;
                    }
                    //กรณีที่มีข้อมูลเก่า id เดียวกันให้ ลบก่อน
                    sqlstr = " update BZ_DOC_IMG set " +
                             " STATUS = " + img_status;
                    if (dtlist[i].pagename.ToString().ToLower() == "visa")
                    {
                        sqlstr += " , ACTIVE_TYPE = " + conn.ChkSqlStr(dtlist[i].active_type, 300);
                    }

                    if (dtlist[i].pagename.ToString().ToLower() == "passport")
                    {
                    }

                    if (action_type == "update")
                    {
                        sqlstr += " ,PATH = " + conn.ChkSqlStr(dtlist[i].path.ToString(), 300) +
                                  " ,FILE_NAME = " + conn.ChkSqlStr(dtlist[i].filename.ToString(), 300);
                    }
                    sqlstr += " ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300) +
                                " ,UPDATE_DATE = sysdate" +
                                " ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300) +
                                " where ID =" + conn.ChkSqlStr(dtlist[i].id, 300);

                    if (dtlist[i].pagename.ToString().ToLower() == "passport" ||
                        dtlist[i].pagename.ToString().ToLower() == "visadocument" ||
                        dtlist[i].pagename.ToString().ToLower() == "visa")
                    {
                        if ((dtlist[i].id_level_1 + "").ToString() != "")
                        {
                            sqlstr += " and ID_LEVEL_1 =" + conn.ChkSqlStr(dtlist[i].id_level_1, 300);
                        }
                        if ((dtlist[i].id_level_2 + "").ToString() != "")
                        {
                            sqlstr += " and ID_LEVEL_2 =" + conn.ChkSqlStr(dtlist[i].id_level_2, 300);
                        }

                        if (dtlist[i].actionname.ToString().ToLower() == "visa_page")
                        {
                            sqlstr += " and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            sqlstr += " and DOC_ID = 'personal'";
                        }
                        else if (dtlist[i].pagename.ToString().ToLower() == "visa")
                        {
                            sqlstr += " and DOC_ID =" + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += " and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                    }
                    else
                    {
                        if (dtlist[i].pagename.ToString().ToLower() == "isos" || dtlist[i].pagename.ToString().ToLower() == "mtvisacountries")
                        {
                        }
                        else
                        {
                            sqlstr += " and DOC_ID =" + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += " and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                    }

                }

                ret = conn_ExecuteNonQuery(sqlstr, true);
                if (ret.ToLower().IndexOf("The operation has timed out") > -1)
                {
                    ret = "true";
                }
                else
                {
                    sqlstr_all += sqlstr + "||";
                }
                if (ret.ToLower() != "true") { break; }

            }

            if (ret.ToLower() == "true")
            {
                //delete file 

            }

            return ret;
        }




        #endregion Function in Doc
        //2. BAPI se37/ZTHROMB005 RFC สำหรับ update ข้อมูลพนักงาน เพื่ออัพเดตข้อมูลการเคลมค่าพาสปอร์ตและกระเป๋าเดินทางเข้า SAP
        public string insertPassort(string emp_id)
        {
            return "true";
            string msg_error = "true";
            try
            {
                //กรณีที่มีการแก้ไขข้อมูลให้ส่งไป update sap 
                sqlstr_all = "";
                sqlstr = @" select passport_no, passport_date_issue, passport_date_expire
                            , to_char(to_date(passport_date_issue,'dd Mon yyyy'),'rrrrMMdd') as sdate
                            , to_char(to_date(passport_date_expire,'dd Mon yyyy'),'rrrrMMdd') as edate
                            from bz_data_passport 
                            where default_type = 'true' and emp_id ='" + emp_id + "' and ( select to_char(usertype) as x from vw_bz_users where employeeid ='" + emp_id + "') = '1' ";
                dt = new DataTable();
                conn = new cls_connection_ebiz();
                if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                {
                    if (dt.Rows.Count > 0)
                    {
                        string sdate = dt.Rows[0]["sdate"].ToString();
                        string edate = dt.Rows[0]["edate"].ToString();

                        sqlstr = @" select count(1) as xcount from bz_passport
                                    where status = 1 
                                    and emp_id ='" + emp_id + "' and passport_no = '" + dt.Rows[0]["passport_no"].ToString() + "' and passport_date_issue = '" + dt.Rows[0]["passport_date_issue"].ToString() + "' and passport_date_expire = '" + dt.Rows[0]["passport_date_expire"].ToString() + "'";
                        dt = new DataTable();
                        conn = new cls_connection_ebiz();
                        if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["xcount"].ToString() != "0") { return "true"; }
                            }
                        }

                        //WS_ZTHRTEB020.SAP_ZTHRTEB020 ws_sap = new WS_ZTHRTEB020.SAP_ZTHRTEB020();
                        //ret = ws_sap.ZTHROMB005_ref(emp_id, sdate, edate);

                        if (ret.ToLower() != "true")
                        {
                            msg_error = " SAP Error :" + ret;
                        }
                        else
                        {
                            //updare ข้อมูล ใน bz_passport
                            msg_error = "";
                            sqlstr = @" update bz_passport set STATUS = 0 where emp_id ='" + emp_id + "'";
                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";
                            if (ret.ToLower() == "true")
                            {
                                sqlstr = @" insert into bz_passport (ID,EMP_ID,PASSPORT_NO,PASSPORT_DATE_ISSUE,PASSPORT_DATE_EXPIRE,PASSPORT_TITLE,PASSPORT_NAME,PASSPORT_SURNAME,PASSPORT_DATE_BIRTH
                                        , CREATE_BY, CREATE_DATE, UPDATE_BY, UPDATE_DATE
                                        , TOKEN_UPDATE, STATUS, SAP_LAST_UPDATE_DATE, SAP_LAST_UPDATE_BY, SAP_STATUS, SAP_REMARK) 
                                        select ID,EMP_ID,PASSPORT_NO,PASSPORT_DATE_ISSUE,PASSPORT_DATE_EXPIRE,PASSPORT_TITLE,PASSPORT_NAME,PASSPORT_SURNAME,PASSPORT_DATE_BIRTH
                                        , CREATE_BY, CREATE_DATE, UPDATE_BY, UPDATE_DATE
                                        , TOKEN_UPDATE ";
                                sqlstr += @" , '1' as STATUS, sysdate as SAP_LAST_UPDATE_DATE, TOKEN_UPDATE as SAP_LAST_UPDATE_BY";
                                sqlstr += @" , 'S' as SAP_STATUS, '" + msg_error + "' as SAP_REMARK";
                                sqlstr += @" from bz_data_passport where default_type = 'true' and emp_id ='" + emp_id + "'";
                                ret = conn_ExecuteNonQuery(sqlstr, true);
                                sqlstr_all += sqlstr + "||";

                                if (ret.ToLower() == "true")
                                {
                                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                                    msg_error = ret;
                                }
                                else { msg_error = " SAP Error1 :" + ret; }

                            }
                        }

                    }
                }

            }
            catch (Exception ex_msg_sap) { msg_error = " SAP Error2 :" + ex_msg_sap; }


            return msg_error;
        }
        internal static int SendDataPassortToSAP(string token_login)
        {
            cls_connection_ebiz conn = new cls_connection_ebiz();
            string ret = "";
            DataTable dtdata = new DataTable();
            int iResult = -1;
            string sqlstr = @" select emp_id, passport_no, passport_date_issue, passport_date_expire, passport_date_birth, passport_title, passport_name, passport_surname  
                               ,case when update_by is null then create_by else update_by end last_update_by 
                               from bz_passport where status = 0 ";
            conn_ExecuteData(ref dtdata, sqlstr);

            if (dtdata.Rows.Count > 0)
            {
                string sqlstr_all = "";
                int istatus = 0;
                string emp_id = "";
                string passport_no = "";
                string last_update_by = "";
                for (int i = 0; i < dtdata.Rows.Count; i++)
                {
                    istatus = 0;
                    emp_id = dtdata.Rows[i]["emp_id"].ToString();
                    passport_no = dtdata.Rows[i]["passport_no"].ToString();
                    last_update_by = dtdata.Rows[i]["last_update_by"].ToString();
                    #region  send to sap 
                    ret = "true";
                    if (ret == "true")
                    {
                        istatus = 1;
                    }
                    #endregion  send to sap 

                    if (istatus == 1)
                    {
                        conn = new cls_connection_ebiz();
                        sqlstr = "update bz_passport  set status = '" + istatus.ToString() + "', sap_last_update_date = sysdate, sap_last_update_by = " + conn.ChkSqlStr(last_update_by, 50) + "" +
                                 " where emp_id = " + emp_id +
                                 " and passport_no =" + conn.ChkSqlStr(passport_no, 50);
                        sqlstr_all += sqlstr + "||";
                    }

                }

                if (sqlstr_all != "")
                {
                    //ret = conn_ExecuteNonQuery(sqlstr_all, true);
                    ret = execute_data_ex(sqlstr_all, false);
                    conn = new cls_connection_ebiz();

                    sqlstr = @"call bz_sp_insert_log('SendDataPassortToSAP', " + conn.ChkSqlStr(ret, 300) + ", null, null, null, null, " + conn.ChkSqlStr(token_login, 300) + ");";
                    //ret = conn_ExecuteNonQuery(sqlstr, true);
                    ret = execute_data_ex(sqlstr, false);
                }
            }

            return iResult;
        }
        internal static string execute_data_ex(string xstring, Boolean type_check)
        {
            //data maximum 100 row
            string ret = "";
            string sqlstr = "";
            string[] drdata = xstring.Split(';');


            DataTable dt = new DataTable();
            cls_connection_ebiz conn = new cls_connection_ebiz();
            conn.OpenConnection();
            conn.BeginTransaction();

            for (int i = 0; i < drdata.Length; i++)
            {
                sqlstr = drdata[i].ToString();
                if (sqlstr == "") { continue; }

                ret = conn.ExecuteNonQuery(sqlstr);
                if (ret.ToLower() != "true") { break; } else { ret = ret.ToLower(); }
            }

            if (ret.ToLower() == "true" && type_check == false)
            {
                conn.CommitTransaction();
            }
            else
            {
                conn.RollbackTransaction();
            }
            conn.CloseConnection();

            return ret;
        }

        #region set data in page

        public TravelerHistoryOutModel SetTravelerHistory(TravelerHistoryOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_id_active = value.emp_id;
            var token_login = data.token_login;
            #region set data 

            int imaxid = 1;
            dt = new DataTable();
            sqlstr = "select (nvl( max(to_number(id)),0)+1)as id from BZ_USER_PEOFILE";
            if (conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        if (dt.Rows[0]["id"].ToString() != "") { imaxid = Convert.ToInt32(dt.Rows[0]["id"].ToString()); }
                    }
                    catch { }
                }
            }

            if (data.traveler_emp.Count > 0)
            {
                conn = new cls_connection_ebiz();

                List<travelerEmpList> dtlist = data.traveler_emp;
                for (int i = 0; i < dtlist.Count; i++)
                {
                    var action_type = dtlist[i].action_type.ToString();
                    if (action_type == "") { continue; }
                    else if (action_type != "delete")
                    {
                        var action_change = dtlist[i].action_change + "";
                        //if (action_change.ToLower() != "true") { continue; }
                    }

                    var doc_id = dtlist[i].doc_id.ToString();
                    var emp_id_select = dtlist[i].emp_id.ToString();

                    action_type = "insert";
                    sqlstr = "select * from  bz_user_peofile where employeeid = '" + emp_id_select + "' ";
                    //if (doc_id != "personal")
                    //{
                    //    sqlstr += " and doc_id = '" + doc_id + "' ";
                    //}

                    DataTable dtEmp = new DataTable();
                    conn = new cls_connection_ebiz();
                    if (SetDocService.conn_ExecuteData(ref dtEmp, sqlstr) == "")
                    {
                        if (dtEmp.Rows.Count > 0)
                        {
                            action_type = "update";
                        }
                    }

                    if (action_type == "insert")
                    {
                        sqlstr = @" insert into BZ_USER_PEOFILE (ID,DOC_ID,EMPLOYEEID,USERID,MOBILE,TELEPHONE
                                    ,IMGPATH,IMGPROFILENAME,CREATE_BY,CREATE_DATE,UPDATE_BY,UPDATE_DATE,TOKEN_UPDATE)" +
                                      " values (" +
                                      " " + imaxid +
                                      " ," + conn.ChkSqlStr(doc_id, 300) +
                                      " ," + conn.ChkSqlStr(dtlist[i].emp_id, 300) +
                                      " ," + conn.ChkSqlStr(dtlist[i].userName, 300) +
                                      " ," + conn.ChkSqlStr(dtlist[i].userPhone, 300) +
                                      " ," + conn.ChkSqlStr(dtlist[i].userTel, 300) +
                                      " ," + conn.ChkSqlStr(dtlist[i].imgpath, 300) +
                                      " ," + conn.ChkSqlStr(dtlist[i].imgprofilename, 300) +
                                      " ," + conn.ChkSqlStr(emp_id_active, 300) +
                                      " ,sysdate,null,null" +
                                      " ," + conn.ChkSqlStr(token_login, 300) +
                                      " )";
                        imaxid++;

                    }
                    else if (action_type == "update")
                    {
                        sqlstr = " update BZ_USER_PEOFILE set " +
                                 " MOBILE = " + conn.ChkSqlStr(dtlist[i].userPhone, 300) +
                                 " ,TELEPHONE = " + conn.ChkSqlStr(dtlist[i].userTel, 300) +
                                 " ,UPDATE_BY =" + conn.ChkSqlStr(token_login, 300) +
                                 " ,UPDATE_DATE = sysdate" +
                                 " ,TOKEN_UPDATE =" + conn.ChkSqlStr(token_login, 300) + "" +
                                 " where EMPLOYEEID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        //if (doc_id != "personal")
                        //{
                        //    sqlstr += " and DOC_ID =" + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                        //}

                    }

                    ret = conn_ExecuteNonQuery(sqlstr, true);
                    sqlstr_all += sqlstr + "||";

                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }

            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public AirTicketOutModel SetAirTicket(AirTicketOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id; 
            var token_login = data.token_login;

            Boolean already_booked_emp_select = false;

            #region set data 
            SearchDocService _swd = new SearchDocService();
            int imaxid = GetMaxID("BZ_DOC_AIRTICKET_BOOKING");
            int imaxidSub = GetMaxID("BZ_DOC_AIRTICKET_DETAIL");
            int imaxidImg = GetMaxID("BZ_DOC_IMG");

            DataTable dtallowance = new DataTable();
            sqlstr = @" select distinct emp_id from bz_doc_allowance where doc_id ='" + value.doc_id + "'  ";
            conn_ExecuteData(ref dtallowance, sqlstr);

            conn = new cls_connection_ebiz();
            if (data.airticket_booking.Count > 0)
            {
                sqlstr_all = "";
                if (data.airticket_booking.Count > 0)
                {
                    List<airticketbookList> dtlist = data.airticket_booking;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                        string doc_status = "";
                        try
                        {
                            doc_status = dtlist[i].doc_status.ToString();
                        }
                        catch { }
                        if (doc_status == "") { doc_status = "1"; }
                        try
                        {
                            if (dtlist[i].booking_status.ToString() != "")
                            {
                                //Traveler หรือ Admin Action : เลือก  confirm + submit 
                                if (doc_type == "submit" && dtlist[i].booking_status.ToString() == "2") { doc_status = "4"; }
                                else if (dtlist[i].booking_status.ToString() == "1" || dtlist[i].booking_status.ToString() == "3"
                                    || doc_type == "save")
                                {
                                    //Admin Action : เลือก Booked + Waiting List + กด Save / Submit  
                                    if (data.user_admin == true)
                                    {
                                        doc_status = "3";
                                    }
                                    else
                                    {
                                        doc_status = "2";
                                    }
                                }
                            }
                        }
                        catch { }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DOC_AIRTICKET_BOOKING
                                    (ID,DOC_ID,DOC_STATUS,EMP_ID,DATA_TYPE,ASK_BOOKING,SEARCH_AIR_TICKET,AS_COMPANY_RECOMMEND,ALREADY_BOOKED,ALREADY_BOOKED_OTHER,ALREADY_BOOKED_ID,BOOKING_REF,BOOKING_STATUS,ADDITIONAL_REQUEST,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(doc_status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].data_type, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ask_booking, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].search_air_ticket, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].as_company_recommend, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].already_booked, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].already_booked_other, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].already_booked_id, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].booking_ref, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].booking_status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].additional_request, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_AIRTICKET_BOOKING set";

                            sqlstr += @" ASK_BOOKING = " + conn.ChkSqlStr(dtlist[i].ask_booking, 300);
                            sqlstr += @" ,SEARCH_AIR_TICKET = " + conn.ChkSqlStr(dtlist[i].search_air_ticket, 300);
                            sqlstr += @" ,AS_COMPANY_RECOMMEND = " + conn.ChkSqlStr(dtlist[i].as_company_recommend, 300);
                            sqlstr += @" ,ALREADY_BOOKED = " + conn.ChkSqlStr(dtlist[i].already_booked, 300);
                            sqlstr += @" ,ALREADY_BOOKED_OTHER = " + conn.ChkSqlStr(dtlist[i].already_booked_other, 4000);
                            sqlstr += @" ,ALREADY_BOOKED_ID = " + conn.ChkSqlStr(dtlist[i].already_booked_id, 4000);
                            sqlstr += @" ,BOOKING_REF = " + conn.ChkSqlStr(dtlist[i].booking_ref, 300);
                            sqlstr += @" ,BOOKING_STATUS = " + conn.ChkSqlStr(dtlist[i].booking_status, 300);
                            sqlstr += @" ,ADDITIONAL_REQUEST = " + conn.ChkSqlStr(dtlist[i].additional_request, 300);
                            sqlstr += @" ,DATA_TYPE = " + conn.ChkSqlStr(dtlist[i].data_type, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);

                            sqlstr += @" ,DOC_STATUS = " + conn.ChkSqlStr(doc_status, 300);

                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_AIRTICKET_BOOKING ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }
                if (data.airticket_detail.Count > 0)
                {
                    List<airticketList> dtlist = data.airticket_detail;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DOC_AIRTICKET_DETAIL
                                    (ID,DOC_ID,EMP_ID,AIRTICKET_DATE,AIRTICKET_ROUTE_FROM,AIRTICKET_ROUTE_TO,AIRTICKET_FLIGHT,AIRTICKET_DEPARTURE_TIME,AIRTICKET_ARRIVAL_TIME,AIRTICKET_DEPARTURE_DATE,AIRTICKET_ARRIVAL_DATE,CHECK_MY_TRIP,AIRTICKET_ROOT
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxidSub;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            //sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_date, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_departure_date, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_route_from, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_route_to, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_flight, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_departure_time, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_arrival_time, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_departure_date, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_arrival_date, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].check_my_trip, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].airticket_root, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxidSub++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_AIRTICKET_DETAIL set";

                            //sqlstr += @" AIRTICKET_DATE = " + conn.ChkSqlStr(dtlist[i].airticket_date, 300);
                            sqlstr += @" AIRTICKET_DATE = " + conn.ChkSqlStr(dtlist[i].airticket_departure_date, 300);
                            sqlstr += @" ,AIRTICKET_ROUTE_FROM = " + conn.ChkSqlStr(dtlist[i].airticket_route_from, 300);
                            sqlstr += @" ,AIRTICKET_ROUTE_TO = " + conn.ChkSqlStr(dtlist[i].airticket_route_to, 300);
                            sqlstr += @" ,AIRTICKET_FLIGHT = " + conn.ChkSqlStr(dtlist[i].airticket_flight, 300);
                            sqlstr += @" ,AIRTICKET_DEPARTURE_TIME = " + conn.ChkSqlStr(dtlist[i].airticket_departure_time, 300);
                            sqlstr += @" ,AIRTICKET_ARRIVAL_TIME = " + conn.ChkSqlStr(dtlist[i].airticket_arrival_time, 300);
                            sqlstr += @" ,AIRTICKET_DEPARTURE_DATE = " + conn.ChkSqlStr(dtlist[i].airticket_departure_date, 300);
                            sqlstr += @" ,AIRTICKET_ARRIVAL_DATE = " + conn.ChkSqlStr(dtlist[i].airticket_arrival_date, 300);
                            sqlstr += @" ,CHECK_MY_TRIP = " + conn.ChkSqlStr(dtlist[i].check_my_trip, 300);
                            sqlstr += @" ,AIRTICKET_ROOT = " + conn.ChkSqlStr(dtlist[i].airticket_root, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_AIRTICKET_DETAIL ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }
                if (data.img_list.Count > 0)
                {
                    ret = "true"; sqlstr = "";
                    ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

                if (data.airticket_booking.Count > 0)
                {
                    List<airticketbookList> dtlist = data.airticket_booking;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        var _data_type = "save";
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        _data_type = dtlist[i].data_type.ToString();

                        if (_data_type == "submit")
                        {
                            #region delelte data กรณีที่เป็น submit
                            sqlstr = @" delete from BZ_DOC_AIRTICKET_DETAIL_KEEP ";
                            sqlstr += @" where ";
                            sqlstr += @" DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }
                            #endregion delelte data กรณีที่เป็น submit

                            #region save data to bz_doc_airticket_detail_keep  
                            sqlstr = @" insert into BZ_DOC_AIRTICKET_DETAIL_KEEP
                                        select a.* 
                                        from BZ_DOC_AIRTICKET_DETAIL a 
                                        inner join bz_doc_airticket_booking b on a.emp_id = b.emp_id and a.doc_id = b.doc_id and b.data_type = 'submit' ";
                            sqlstr += @" where ";
                            sqlstr += @" a.DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and a.EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }
                            #endregion save data to bz_doc_airticket_detail_keep 

                        }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }
            }
            #endregion set data

            var msg_error = "";
            var msg_status = "Save data";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                if (doc_type == "submit")
                {
                    if (true)
                    {
                        if (dtallowance.Rows.Count > 0)
                        {
                            if (data.airticket_booking.Count > 0)
                            {
                                List<airticketbookList> dtlist = data.airticket_booking;
                                for (int i = 0; i < dtlist.Count; i++)
                                {
                                    ret = "true"; sqlstr = "";
                                    var action_type = dtlist[i].action_type.ToString();
                                    var data_type = dtlist[i].data_type.ToString();
                                    var doc_id_select = dtlist[i].doc_id.ToString();
                                    var emp_id_select = dtlist[i].emp_id.ToString();
                                    if (action_type == "") { continue; }
                                    else if (action_type != "delete")
                                    {
                                        var action_change = dtlist[i].action_change + "";
                                        if (action_change.ToLower() != "true") { continue; }
                                    }
                                    if (data_type == "submit")
                                    {
                                        //ตรวจสอบว่าเป็นการ submit ให้คำนวณค่า allowance ใหม่ เฉพาะที่มีข้อมูล allowance
                                        if (dtallowance.Rows.Count > 0)
                                        {

                                            DataRow[] drcheck = dtallowance.Select("emp_id='" + emp_id_select + "'");
                                            if (drcheck.Length > 0)
                                            {

                                                sqlstr = " delete from bz_doc_allowance_detail where doc_id='" + doc_id_select + "' and emp_id='" + emp_id_select + "' ";
                                                ret = conn_ExecuteNonQuery(sqlstr, true);
                                                sqlstr_all += sqlstr + "||";

                                                #region Auwat 20210826 0000 update ข้อมูล doc status ALLOWANCE

                                                string passport = "";
                                                string passport_date = "";
                                                string luggage_clothing = "";
                                                string luggage_clothing_date = "";

                                                try
                                                {
                                                    _swd = new SearchDocService();
                                                    var est = _swd.EstimateExpense(doc_id_select, emp_id_select);
                                                    if (est.PassportExpense.ToString() != "")
                                                    {
                                                        passport = est.PassportExpense.ToString();
                                                    }
                                                    if (est.PassportDate.ToString() != "")
                                                    {
                                                        passport_date = _swd.convert_date_display(est.PassportDate.ToString());
                                                    }
                                                    if (est.CLExpense.ToString() != "")
                                                    {
                                                        luggage_clothing = est.CLExpense.ToString();
                                                    }
                                                    if (est.CLDate.ToString() != "")
                                                    {
                                                        luggage_clothing_date = _swd.convert_date_display(est.CLDate.ToString());//2021-03-11
                                                    }
                                                }
                                                catch { }

                                                sqlstr = @" update BZ_DOC_ALLOWANCE set";
                                                sqlstr += @" DOC_STATUS = " + conn.ChkSqlStr("1", 20);
                                                if (passport.ToString() != "")
                                                {
                                                }
                                                if (passport_date.ToString() != "")
                                                {
                                                }
                                                if (luggage_clothing.ToString() != "")
                                                {
                                                    sqlstr += @" ,LUGGAGE_CLOTHING = " + conn.ChkSqlStr(luggage_clothing, 300);
                                                }
                                                if (luggage_clothing_date.ToString() != "")
                                                {
                                                    sqlstr += @" ,LUGGAGE_CLOTHING_DATE = " + conn.ChkSqlStr(luggage_clothing_date, 300);
                                                }
                                                sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                                sqlstr += @" ,UPDATE_DATE = sysdate";
                                                sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                                                sqlstr += @" where ";
                                                sqlstr += @" DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                                ret = conn_ExecuteNonQuery(sqlstr, true);
                                                sqlstr_all += sqlstr + "||";

                                                if (ret == "true")
                                                {
                                                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                                                }
                                                #endregion Auwat 20210826 0000 update ข้อมูล doc status ALLOWANCE 
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }

                    msg_status = "Submit Data";
                    var page_name = "airticket";
                    var module_name = doc_type;
                    var email_admin = "";
                    var email_user_in_doc = "";
                    var mail_cc_active = "";
                    var role_type = "pmsv_admin";
                    var emp_id_user_in_doc = "";
                    var email_user_display = "";
                    var email_attachments = "";

                    List<EmpListOutModel> emp_list = data.emp_list;
                    List<mailselectList> mail_list = new List<mailselectList>();

                    _swd = new SearchDocService();
                    DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin += dtemplist.Rows[i]["email"] + ";";
                    }
                    if (value.doc_id.ToString().IndexOf("T") > -1)
                    {
                        _swd = new SearchDocService();
                        dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                        for (int i = 0; i < dtemplist.Rows.Count; i++)
                        {
                            email_admin += dtemplist.Rows[i]["email"] + ";";
                        }
                    }

                    //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
                    mail_cc_active = sqlEmpUserMail(value.token_login);

                    List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck.Count > 0)
                    {
                        emp_id_user_in_doc = drempcheck[0].emp_id.ToString();
                        email_user_in_doc = drempcheck[0].userEmail.ToString();
                        email_user_display = drempcheck[0].userDisplay.ToString();
                    }

                    List<airticketbookList> company_rec = data.airticket_booking.Where(a => (a.emp_id == emp_id_user_in_doc)).ToList();
                    string module_name_select = "";
                    if (data.user_admin == true)
                    {
                        if (company_rec != null && company_rec.Count > 0)
                        {
                            if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "1")
                            {
                                module_name_select = "traveler_review";  //032 
                            }
                            else if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "2")
                            {
                                module_name_select = "admin_confirmed";//011 
                                try
                                {
                                    List<ImgList> drimgcheck = data.img_list.Where(a => (a.emp_id == emp_id_user_in_doc) && a.action_type != "delete").ToList();
                                    for (int i = 0; i < drimgcheck.Count; i++)
                                    {
                                        if (email_attachments != "") { email_attachments += ";"; }
                                        email_attachments += drimgcheck[i].fullname;
                                    }
                                }
                                catch { }
                            }
                            else if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "3")
                            {
                                module_name_select = "admin_not_confirmed";//010 
                            }

                            if (module_name_select == "admin_confirmed" || module_name_select == "admin_not_confirmed")
                            {
                                mail_list.Add(new mailselectList
                                {
                                    module = "Air Ticket",
                                    mail_to = email_user_in_doc,
                                    mail_to_display = email_user_display,
                                    mail_cc = email_admin,
                                    mail_attachments = email_attachments,
                                    mail_body_in_form = "",
                                    mail_status = "true",
                                    action_change = "true",
                                    emp_id = emp_id_user_in_doc,
                                });
                            }
                            else
                            {
                                mail_list.Add(new mailselectList
                                {
                                    module = "Air Ticket",
                                    mail_to = email_admin,
                                    mail_to_display = email_user_display,
                                    mail_cc = email_user_in_doc,
                                    mail_attachments = email_attachments,
                                    mail_body_in_form = "",
                                    mail_status = "true",
                                    action_change = "true",
                                    emp_id = emp_id_user_in_doc,
                                });
                            }

                        }
                    }
                    else
                    {
                        if (company_rec != null && company_rec.Count > 0)
                        {
                            try
                            {
                                //20211108 1438 ของเดิมเช็ค As Company Recommend เปลี่ยนเป็นเช็คแค่ Ask Booking by Company 
                                if (company_rec[0].ask_booking.ToString() == "true")
                                {
                                    module_name_select = "traveler_request";//009 
                                }
                                else if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "1")
                                {
                                    //Already Booked, Booked  

                                    //TO: Admin - PMSV; Admin - PMDV(if any) ;
                                    //CC: Traveler 
                                    module_name_select = "traveler_review";//032 
                                }

                                mail_list.Add(new mailselectList
                                {
                                    module = "Air Ticket",
                                    mail_to = email_admin,
                                    mail_to_display = email_user_display,
                                    mail_cc = email_user_in_doc,
                                    mail_attachments = email_attachments,
                                    mail_body_in_form = "",
                                    mail_status = "true",
                                    action_change = "true",
                                    emp_id = emp_id_user_in_doc,
                                });
                            }
                            catch { }
                        }
                    }

                    if (module_name_select != "")
                    {
                        ret = "";
                        SendEmailService swmail = new SendEmailService();
                        ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id, page_name, module_name_select);
                        if (ret.ToLower() != "true")
                        {
                            msg_error = ret;
                        }
                    }


                }

                SearchDocService swd = new SearchDocService();
                AirTicketModel value_load = new AirTicketModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new AirTicketOutModel();
                data = swd.SearchAirTicket(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? msg_status + " succesed." : msg_status + " failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }

        public AccommodationOutModel SetAccommodation(AccommodationOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;
            Boolean already_booked_emp_select = false;


            var msg_ex_def = "";
            ret = "";
            #region set data 

            if (data.accommodation_booking.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DOC_ACCOMMODATION_BOOKING");
                int imaxidSub = GetMaxID("BZ_DOC_ACCOMMODATION_DETAIL");
                int imaxidImg = GetMaxID("BZ_DOC_IMG");

                try
                {
                    if (data.accommodation_booking.Count > 0)
                    {
                        List<accommodationbookList> dtlist = data.accommodation_booking;
                        for (int i = 0; i < dtlist.Count; i++)
                        {
                            conn = new cls_connection_ebiz();
                            ret = "true";
                            var action_type = dtlist[i].action_type.ToString();
                            if (action_type == "") { continue; }
                            else if (action_type != "delete")
                            {
                                var action_change = dtlist[i].action_change + "";
                                if (action_change.ToLower() != "true") { continue; }
                            }

                            //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                            string doc_status = "";
                            try
                            {
                                doc_status = dtlist[i].doc_status.ToString();
                            }
                            catch { }
                            try
                            {
                                if (dtlist[i].booking_status.ToString() != "")
                                {
                                    //Traveler หรือ Admin Action : เลือก  confirm + submit 
                                    if (doc_type == "submit" && dtlist[i].booking_status.ToString() == "2") { doc_status = "4"; }
                                    else if (dtlist[i].booking_status.ToString() == "1" || dtlist[i].booking_status.ToString() == "3" || doc_type == "save")
                                    {
                                        //Admin Action : เลือก Booked + Waiting List + กด Save / Submit  
                                        if (data.user_admin == true)
                                        {
                                            doc_status = "3";
                                        }
                                        else
                                        {
                                            doc_status = "2";
                                        }
                                    }
                                }
                            }
                            catch { }

                            try
                            {
                                List<EmpListOutModel> drempcheck = data.emp_list.Where(a => ((a.mail_status == "true") && (a.emp_id == dtlist[i].emp_id.ToString()))).ToList();
                                if (drempcheck.Count > 0)
                                {
                                    drempcheck[0].doc_status_id = doc_status.ToString();
                                    if (dtlist[i].booking_status.ToString() == "1" || dtlist[i].booking_status.ToString() == "3")
                                    { already_booked_emp_select = false; }
                                    else { already_booked_emp_select = true; }
                                    try
                                    {
                                        if (data.user_admin == true && dtlist[i].already_booked.ToString() == "false") { already_booked_emp_select = true; }
                                    }
                                    catch { }
                                }
                            }
                            catch { }




                            if (action_type == "insert")
                            {
                                sqlstr = @" insert into  BZ_DOC_ACCOMMODATION_BOOKING
                                    (ID,DOC_ID,EMP_ID,BOOKING,SEARCH,RECOMMEND,ALREADY_BOOKED,ALREADY_BOOKED_OTHER,ALREADY_BOOKED_ID,ADDITIONAL_REQUEST,BOOKING_STATUS,PLACE_NAME,MAP_URL
                                    ,DOC_STATUS,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                                sqlstr += @" " + imaxid;
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].booking, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].search, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].recommend, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].already_booked, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].already_booked_other, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].already_booked_id, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].additional_request, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].booking_status, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].place_name, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].map_url, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(doc_status, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,sysdate";
                                sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" )";

                                imaxid++;
                            }
                            else if (action_type == "update")
                            {
                                sqlstr = @" update BZ_DOC_ACCOMMODATION_BOOKING set";

                                sqlstr += @" BOOKING = " + conn.ChkSqlStr(dtlist[i].booking, 300);
                                sqlstr += @" ,SEARCH = " + conn.ChkSqlStr(dtlist[i].search, 300);
                                sqlstr += @" ,RECOMMEND = " + conn.ChkSqlStr(dtlist[i].recommend, 300);
                                sqlstr += @" ,ALREADY_BOOKED = " + conn.ChkSqlStr(dtlist[i].already_booked, 300);
                                sqlstr += @" ,ALREADY_BOOKED_OTHER = " + conn.ChkSqlStr(dtlist[i].already_booked_other, 4000);
                                sqlstr += @" ,ALREADY_BOOKED_ID = " + conn.ChkSqlStr(dtlist[i].already_booked_id, 4000);
                                sqlstr += @" ,ADDITIONAL_REQUEST = " + conn.ChkSqlStr(dtlist[i].additional_request, 300);
                                sqlstr += @" ,BOOKING_STATUS = " + conn.ChkSqlStr(dtlist[i].booking_status, 300);
                                sqlstr += @" ,PLACE_NAME = " + conn.ChkSqlStr(dtlist[i].place_name, 300);
                                sqlstr += @" ,MAP_URL = " + conn.ChkSqlStr(dtlist[i].map_url, 300);
                                sqlstr += @" ,DOC_STATUS = " + conn.ChkSqlStr(doc_status, 300);

                                sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,UPDATE_DATE = sysdate";
                                sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            }
                            else if (action_type == "delete")
                            {
                                sqlstr = @" delete from BZ_DOC_ACCOMMODATION_BOOKING ";
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            }
                            //ret = conn.ExecuteNonQuery(sqlstr); 
                            //if (ret.ToLower() != "true") { goto Next_line_1; }

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }


                        }
                    }

                    if (data.accommodation_detail.Count > 0)
                    {
                        List<accommodationList> dtlist = data.accommodation_detail;
                        for (int i = 0; i < dtlist.Count; i++)
                        {
                            ret = "true";
                            var action_type = dtlist[i].action_type.ToString();
                            if (action_type == "") { continue; }
                            else if (action_type != "delete")
                            {
                                var action_change = dtlist[i].action_change + "";
                                if (action_change.ToLower() != "true") { continue; }
                            }


                            if (action_type == "insert")
                            {
                                sqlstr = @" insert into  BZ_DOC_ACCOMMODATION_DETAIL
                                    (ID,DOC_ID,EMP_ID,COUNTRY,HOTEL_NAME,CHECK_IN,CHECK_OUT,ROOMTYPE
                                     ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                                sqlstr += @" " + imaxidSub;
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].country, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].hotel_name, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].check_in, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].check_out, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].roomtype, 300);

                                sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,sysdate";
                                sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" )";

                                imaxidSub++;
                            }
                            else if (action_type == "update")
                            {
                                sqlstr = @" update BZ_DOC_ACCOMMODATION_DETAIL set";

                                sqlstr += @" COUNTRY = " + conn.ChkSqlStr(dtlist[i].country, 300);
                                sqlstr += @" ,HOTEL_NAME = " + conn.ChkSqlStr(dtlist[i].hotel_name, 300);
                                sqlstr += @" ,CHECK_IN = " + conn.ChkSqlStr(dtlist[i].check_in, 300);
                                sqlstr += @" ,CHECK_OUT = " + conn.ChkSqlStr(dtlist[i].check_out, 300);
                                sqlstr += @" ,ROOMTYPE = " + conn.ChkSqlStr(dtlist[i].roomtype, 300);

                                sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,UPDATE_DATE = sysdate";
                                sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            }
                            else if (action_type == "delete")
                            {
                                sqlstr = @" delete from BZ_DOC_ACCOMMODATION_DETAIL ";
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            }
                            //ret = conn.ExecuteNonQuery(sqlstr); 
                            //if (ret.ToLower() != "true") { goto Next_line_1; }

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }

                        }
                    }
                    if (data.img_list.Count > 0)
                    {
                        ret = "true"; sqlstr = "";
                        ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                        if (ret.ToLower() != "true") { goto Next_line_1; }
                    }
                }
                catch (Exception ex_def) { msg_ex_def = ex_def.Message.ToString() + " Sql " + sqlstr; }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }
            }
            #endregion set data

            var msg_error = "";
            var msg_status = "Save data";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                if (doc_type == "submit")
                {
                    msg_status = "Submit Data";
                    var page_name = "accommodation";
                    var module_name = doc_type;
                    var email_admin = "";
                    var email_user_in_doc = "";
                    var mail_cc_active = "";
                    var role_type = "pmsv_admin";
                    var emp_id_user_in_doc = "";
                    var email_user_display = "";
                    var email_attachments = "";

                    List<EmpListOutModel> emp_list = data.emp_list;
                    List<mailselectList> mail_list = new List<mailselectList>();

                    SearchDocService _swd = new SearchDocService();
                    DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin += dtemplist.Rows[i]["email"] + ";";
                    }
                    if (value.doc_id.ToString().IndexOf("T") > -1)
                    {
                        _swd = new SearchDocService();
                        dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                        for (int i = 0; i < dtemplist.Rows.Count; i++)
                        {
                            email_admin += dtemplist.Rows[i]["email"] + ";";
                        }
                    }

                    //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
                    mail_cc_active = sqlEmpUserMail(value.token_login);

                    List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck.Count > 0)
                    {
                        emp_id_user_in_doc = drempcheck[0].emp_id.ToString();
                        email_user_in_doc = drempcheck[0].userEmail.ToString();
                        email_user_display = drempcheck[0].userDisplay.ToString();
                    }

                    List<accommodationbookList> company_rec = data.accommodation_booking.Where(a => (a.emp_id == emp_id_user_in_doc)).ToList();
                    string module_name_select = "";
                    if (data.user_admin == true)
                    {
                        if (company_rec != null && company_rec.Count > 0)
                        {
                            if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "1")
                            {
                                module_name_select = "traveler_review";  //032 
                            }
                            else if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "2")
                            {
                                module_name_select = "admin_confirmed";//011 
                                try
                                {
                                    List<ImgList> drimgcheck = data.img_list.Where(a => (a.emp_id == emp_id_user_in_doc)).ToList();
                                    for (int i = 0; i < drimgcheck.Count; i++)
                                    {
                                        if (email_attachments != "") { email_attachments += ";"; }
                                        email_attachments += drimgcheck[i].fullname;
                                    }
                                }
                                catch { }
                            }
                            else if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "3")
                            {
                                module_name_select = "admin_not_confirmed";//010 
                            }

                            if (module_name_select == "admin_confirmed" || module_name_select == "admin_not_confirmed")
                            {
                                mail_list.Add(new mailselectList
                                {
                                    module = "Accommodation",
                                    mail_to = email_user_in_doc,
                                    mail_to_display = email_user_display,
                                    mail_cc = email_admin,
                                    mail_attachments = email_attachments,
                                    mail_body_in_form = "",
                                    mail_status = "true",
                                    action_change = "true",
                                    emp_id = emp_id_user_in_doc,
                                });
                            }
                            else
                            {
                                mail_list.Add(new mailselectList
                                {
                                    module = "Accommodation",
                                    mail_to = email_admin,
                                    mail_to_display = email_user_display,
                                    mail_cc = email_user_in_doc,
                                    mail_attachments = email_attachments,
                                    mail_body_in_form = "",
                                    mail_status = "true",
                                    action_change = "true",
                                    emp_id = emp_id_user_in_doc,
                                });
                            }

                        }
                    }
                    else
                    {
                        if (company_rec != null && company_rec.Count > 0)
                        {
                            try
                            {
                                //20211108 1438 ของเดิมเช็ค As Company Recommend เปลี่ยนเป็นเช็คแค่ Ask Booking by Company 
                                //if (company_rec[0].recommend.ToString() == "true")
                                if (company_rec[0].booking.ToString() == "true")
                                {
                                    module_name_select = "traveler_request";//009 
                                }
                                else if (company_rec[0].already_booked.ToString() == "true" && company_rec[0].booking_status.ToString() == "1")
                                {
                                    //Already Booked, Booked  
                                    //TO: Admin - PMSV; Admin - PMDV(if any) ;
                                    //CC: Traveler 
                                    module_name_select = "traveler_review";//032 
                                }
                                mail_list.Add(new mailselectList
                                {
                                    module = "Accommodation",
                                    mail_to = email_admin,
                                    mail_to_display = email_user_display,
                                    mail_cc = email_user_in_doc,
                                    mail_attachments = email_attachments,
                                    mail_body_in_form = "",
                                    mail_status = "true",
                                    action_change = "true",
                                    emp_id = emp_id_user_in_doc,
                                });
                            }
                            catch { }
                        }
                    }

                    if (module_name_select != "")
                    {
                        ret = "";
                        SendEmailService swmail = new SendEmailService();
                        ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id, page_name, module_name_select);
                        if (ret.ToLower() != "true")
                        {
                            msg_error = ret;
                        }
                    }
                }

                SearchDocService swd = new SearchDocService();
                AccommodationModel value_load = new AccommodationModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new AccommodationOutModel();
                data = swd.SearchAccommodation(value_load);

            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? msg_status + " succesed." : msg_status + " failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;



            return data;
        }
        public VisaOutModel SetVisa(VisaOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data 
            Boolean user_admin = false;
            string user_id = "";
            string user_role = "";
            sqlEmpRole(token_login, ref user_id, ref user_role, ref user_admin, data.doc_id);
            emp_user_active = user_id;

            string visa_card_id = "";
            if (data.visa_detail.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DATA_VISA");
                int imaxidImg = GetMaxID("BZ_DOC_IMG");

                conn = new cls_connection_ebiz();

                if (data.visa_detail.Count > 0)
                {
                    List<visaList> dtlist = data.visa_detail;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        Boolean bCheckDoc = false;
                        DataTable dtdoc_check = new DataTable();
                        if ((data.doc_id + "") != "personal")
                        {
                            sqlstr = @"select count(1) as xcount from  BZ_DOC_VISA where doc_id = '" + dtlist[i].doc_id.ToString() + "'";
                            sqlstr += " and emp_id = '" + dtlist[i].emp_id.ToString() + "' ";
                            conn = new cls_connection_ebiz();
                            if (SetDocService.conn_ExecuteData(ref dtdoc_check, sqlstr) == "")
                            {
                                if (dtdoc_check != null)
                                {
                                    if (dtdoc_check.Rows.Count > 0)
                                    {
                                        if (dtdoc_check.Rows[0]["xcount"].ToString() != "0") { bCheckDoc = true; }
                                    }
                                }
                            }
                        }

                        ret = "true";
                        var id_def = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true")
                            {
                                if (action_type == "update" && bCheckDoc == false)
                                {
                                    action_type = "insert";
                                    id_def = imaxid.ToString();
                                    imaxid++;
                                    goto Next_line_0;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into BZ_DATA_VISA
                                    (ID,DOC_ID,EMP_ID,VISA_PLACE_ISSUE,VISA_VALID_FROM
                                    ,VISA_VALID_TO,VISA_VALID_UNTIL,VISA_TYPE,VISA_CATEGORY,VISA_ENTRY,VISA_NAME,VISA_SURNAME
                                    ,VISA_DATE_BIRTH,VISA_NATIONALITY,PASSPORT_NO,VISA_SEX,VISA_AUTHORIZED_SIGNATURE,VISA_REMARK,VISA_CARD_ID,VISA_SERIAL
                                    ,DEFAULT_TYPE
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก visa ให้ map กับ emp id
                            //sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr("", 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_place_issue, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_valid_from, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_valid_to, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_valid_until, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_type, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_category, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_entry, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_surname, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_date_birth, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_nationality, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_no, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_sex, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_authorized_signature, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_remark, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_card_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_serial, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].default_type, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";


                            //กรณีที่เป็นข้อมูลใหม่ ให้ map id ใหม่ให้กับ Img ด้วย  
                            if (data.img_list.Count > 0)
                            {
                                List<ImgList> drimg = data.img_list.Where(a => (a.id_level_1 == dtlist[i].id & a.emp_id == dtlist[i].emp_id)).ToList();
                                if (drimg.Count > 0)
                                {
                                    drimg[0].id_level_1 = imaxid.ToString();
                                }
                            }


                            id_def = imaxid.ToString();
                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DATA_VISA set";

                            sqlstr += @" VISA_PLACE_ISSUE = " + conn.ChkSqlStr(dtlist[i].visa_place_issue, 300);
                            sqlstr += @" ,VISA_VALID_FROM = " + conn.ChkSqlStr(dtlist[i].visa_valid_from, 300);
                            sqlstr += @" ,VISA_VALID_TO = " + conn.ChkSqlStr(dtlist[i].visa_valid_to, 300);
                            sqlstr += @" ,VISA_VALID_UNTIL = " + conn.ChkSqlStr(dtlist[i].visa_valid_until, 300);
                            sqlstr += @" ,VISA_TYPE = " + conn.ChkSqlStr(dtlist[i].visa_type, 300);
                            sqlstr += @" ,VISA_CATEGORY = " + conn.ChkSqlStr(dtlist[i].visa_category, 300);
                            sqlstr += @" ,VISA_ENTRY = " + conn.ChkSqlStr(dtlist[i].visa_entry, 300);
                            sqlstr += @" ,VISA_NAME = " + conn.ChkSqlStr(dtlist[i].visa_name, 300);
                            sqlstr += @" ,VISA_SURNAME = " + conn.ChkSqlStr(dtlist[i].visa_surname, 300);
                            sqlstr += @" ,VISA_DATE_BIRTH = " + conn.ChkSqlStr(dtlist[i].visa_date_birth, 300);
                            sqlstr += @" ,VISA_NATIONALITY = " + conn.ChkSqlStr(dtlist[i].visa_nationality, 300);
                            sqlstr += @" ,PASSPORT_NO = " + conn.ChkSqlStr(dtlist[i].passport_no, 300);
                            sqlstr += @" ,VISA_SEX = " + conn.ChkSqlStr(dtlist[i].visa_sex, 300);
                            sqlstr += @" ,VISA_AUTHORIZED_SIGNATURE = " + conn.ChkSqlStr(dtlist[i].visa_authorized_signature, 300);
                            sqlstr += @" ,VISA_REMARK = " + conn.ChkSqlStr(dtlist[i].visa_remark, 300);
                            sqlstr += @" ,VISA_CARD_ID = " + conn.ChkSqlStr(dtlist[i].visa_card_id, 300);
                            sqlstr += @" ,VISA_SERIAL = " + conn.ChkSqlStr(dtlist[i].visa_serial, 300);

                            if ((dtlist[i].default_action_change + "").ToString() == "true")
                            {
                                sqlstr += @" ,DEFAULT_TYPE = " + conn.ChkSqlStr(dtlist[i].default_type, 300);
                            }

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก visa ให้ map กับ emp id
                            //sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            id_def = dtlist[i].id.ToString();

                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DATA_VISA ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก visa ให้ map กับ emp id
                            //sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            id_def = dtlist[i].id.ToString();
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_VISA ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก visa ให้ map กับ emp id
                            //sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";
                        }

                    //เพิ่มข้อมูลใน bz_doc_visa
                    Next_line_0:;
                        if (action_type == "insert" || action_type == "update")
                        {
                            if ((data.doc_id + "") != "personal")
                            {
                                if ((dtlist[i].default_action_change + "").ToString() == "true"
                                    || bCheckDoc == false)
                                {
                                    sqlstr = @" delete from BZ_DOC_VISA ";
                                    sqlstr += @" where ";
                                    sqlstr += @" ID = " + conn.ChkSqlStr(id_def, 300);
                                    sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                    sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                    ret = conn_ExecuteNonQuery(sqlstr, true);
                                    sqlstr_all += sqlstr + "||";

                                    sqlstr = @" insert into BZ_DOC_VISA
                                    (ID,DOC_ID,EMP_ID,VISA_PLACE_ISSUE,VISA_VALID_FROM
                                    ,VISA_VALID_TO,VISA_VALID_UNTIL,VISA_TYPE,VISA_CATEGORY,VISA_ENTRY,VISA_NAME,VISA_SURNAME
                                    ,VISA_DATE_BIRTH,VISA_NATIONALITY,PASSPORT_NO,VISA_SEX,VISA_AUTHORIZED_SIGNATURE,VISA_REMARK,VISA_CARD_ID,VISA_SERIAL
                                    ,DEFAULT_TYPE
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                                    sqlstr += @" " + id_def;
                                    //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก visa ให้ map กับ emp id
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_place_issue, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_valid_from, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_valid_to, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_valid_until, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_type, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_category, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_entry, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_name, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_surname, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_date_birth, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_nationality, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_no, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_sex, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_authorized_signature, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_remark, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_card_id, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_serial, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].default_type, 300);

                                    sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                    sqlstr += @" ,sysdate";
                                    sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                                    sqlstr += @" )";

                                    ret = conn_ExecuteNonQuery(sqlstr, true);
                                    sqlstr_all += sqlstr + "||";

                                }
                            }
                            else
                            {
                                //ถ้าแก้ก็จะไปมีผลต่อใบงานใหม่ หรือใบงานที่ยังไม่เคย save ใบไหนเคย save แล้วไม่ต้องสนใจการแก้ไข 
                            }
                        }

                    }
                }
                if (data.img_list.Count > 0)
                {
                    ret = "true"; sqlstr = "";
                    ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }
            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }
            }

            //update doc status //เนื่องจาก update ทีละ emp id อยุ่แล้ว 
            if (data.visa_detail.Count > 0)
            {
                string emp_id_select = "";
                string doc_status = "";
                List<EmpListOutModel> drempcheck = data.emp_list.Where(a => (a.mail_status == "true")).ToList();
                for (int i = 0; i < drempcheck.Count; i++)
                {
                    emp_id_select = drempcheck[i].emp_id.ToString();
                    try
                    {
                        doc_status = drempcheck[i].doc_status_id.ToString();
                    }
                    catch { }
                }

                if (doc_type == "submit" || doc_type == "sendmail_visa_requisition")
                {
                    if (value.user_admin == true)
                    {
                        sqlstr = @"update BZ_DOC_VISA set DOC_STATUS = '3' where doc_id ='" + data.doc_id + "' and emp_id = '" + emp_id_select + "' ";
                        ret = conn_ExecuteNonQuery(sqlstr, false);
                    }
                }
                else
                {
                    List<visaList> dtlist = data.visa_detail.Where(a => (a.emp_id == emp_id_select && a.visa_active_in_doc == "true")).ToList();
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        if (visa_card_id != "") { visa_card_id += ","; }
                        visa_card_id += "'" + dtlist[i].visa_card_id.ToString() + "'";
                    }

                    Boolean bCheckStatus = false;
                    if (doc_status == "") { doc_status = "0"; }

                    sqlstr = @"  select case when to_date(visa_valid_until,'dd Mon yyyy') >= sysdate then '1' else '0' end  check_data, emp_id 
                                 from BZ_DATA_VISA 
                                 where emp_id = '" + emp_id_select + "' and  visa_card_id in (" + visa_card_id + ")";
                    DataTable dtref = new DataTable();
                    if (SetDocService.conn_ExecuteData(ref dtref, sqlstr) == "")
                    {
                        if (dtref.Rows.Count > 0)
                        {
                            DataRow[] drcheck = dtref.Select("check_data ='0'");
                            if (drcheck.Length == 0)
                            {
                                doc_status = "4";
                                bCheckStatus = true;
                            }
                        }
                    }
                    if (bCheckStatus == false)
                    {
                        if (data.user_admin == true)
                        {
                            doc_status = "3";
                        }
                        else
                        {
                            doc_status = "2";
                        }
                    }

                    sqlstr = @"update BZ_DOC_VISA set DOC_STATUS = '" + doc_status + "' where doc_id ='" + data.doc_id + "' and emp_id = '" + emp_id_select + "' ";
                    //sqlstr = @"update BZ_DATA_VISA set DOC_STATUS = '" + doc_status + "' where emp_id = '" + emp_id_select + "' and VISA_CARD_ID in (" + visa_card_id + ")";
                    ret = conn_ExecuteNonQuery(sqlstr, false);

                }
            }

            #endregion set data

            var msg_error = "";
            var msg_status = "Save data";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                if (doc_type == "submit")
                {
                    msg_status = "Submit Data";
                }
                else
                {
                    msg_status = "Save";
                }

                if ((data.user_admin == false && doc_type == "save")
                    || (data.user_admin == true && doc_type == "submit")
                    || doc_type == "sendmail_visa_requisition")
                {
                    if (data.doc_id != "personal")
                    {
                        //Auwat 20210630 1200 แก้ไขเนื่องจาก font แจ้งมาว่าไม่ได้ใช้งานเส้นนี้ในการส่ง mail Visa Requisition จะใช้ SendMailVisa
                        //doc_type == "sendmail_visa_requisition" 
                        var page_name = "VISA";
                        var module_name = doc_type;
                        var email_admin = "";
                        var email_user_in_doc = "";
                        var mail_cc_active = "";
                        var role_type = "pmsv_admin";
                        var emp_id_user_in_doc = "";
                        var email_user_display = "";
                        var email_attachments = "";

                        if (doc_type == "save") { if (data.user_admin == false) { module_name = "sendmail_visa_employee_letter"; } }
                        if (doc_type == "submit") { if (data.user_admin == true) { module_name = "sendmail_visa_requisition"; } }

                        List<EmpListOutModel> emp_list = data.emp_list;
                        List<mailselectList> mail_list = data.mail_list;
                        SearchDocService _swd = new SearchDocService();
                        DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                        for (int i = 0; i < dtemplist.Rows.Count; i++)
                        {
                            email_admin += dtemplist.Rows[i]["email"] + ";";
                        }
                        if (value.doc_id.ToString().IndexOf("T") > -1)
                        {
                            _swd = new SearchDocService();
                            dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                            for (int i = 0; i < dtemplist.Rows.Count; i++)
                            {
                                email_admin += dtemplist.Rows[i]["email"] + ";";
                            }
                        }

                        //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
                        mail_cc_active = sqlEmpUserMail(value.token_login);

                        List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                        if (drempcheck.Count > 0)
                        {
                            emp_id_user_in_doc = drempcheck[0].emp_id.ToString();
                            email_user_in_doc = drempcheck[0].userEmail.ToString();
                            email_user_display = drempcheck[0].userDisplay.ToString();
                        }

                        mail_list = new List<mailselectList>();
                        if (module_name == "sendmail_visa_requisition")
                        {
                            mail_list = data.mail_list.Where(a => ((a.emp_id.ToLower() == emp_id_user_in_doc))).ToList();
                            mail_list[0].module = "VISA Requisition";
                            mail_list[0].mail_to += email_admin;
                            mail_list[0].mail_to_display = email_user_display;
                            mail_list[0].mail_cc += email_user_in_doc;
                            mail_list[0].mail_body_in_form = "";
                            mail_list[0].mail_status = "true";
                            mail_list[0].action_change = "true";
                            mail_list[0].emp_id = emp_id_user_in_doc;

                        }
                        else if (module_name == "sendmail_visa_employee_letter")
                        {
                            mail_list = data.mail_list.Where(a => ((a.emp_id.ToLower() == emp_id_user_in_doc))).ToList();
                            mail_list[0].module = "VISA";
                            mail_list[0].mail_to += email_admin;
                            mail_list[0].mail_to_display = email_user_display;
                            mail_list[0].mail_cc += email_user_in_doc;
                            mail_list[0].mail_body_in_form = "";
                            mail_list[0].mail_status = "true";
                            mail_list[0].action_change = "true";
                            mail_list[0].emp_id = emp_id_user_in_doc;
                        }

                        ret = "";
                        SendEmailService swmail = new SendEmailService();
                        ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id, page_name, module_name);
                        if (ret.ToLower() != "true")
                        {
                            msg_error = ret;
                        }
                        else
                        {
                            if (module_name == "sendmail_visa_requisition")
                            {
                                msg_status = "Send Visa Requisition Completed";
                            }
                            else
                            {
                                msg_status = "Send Completed";
                            }
                        }
                    }
                }

                SearchDocService swd = new SearchDocService();
                VisaModel value_load = new VisaModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new VisaOutModel();
                data = swd.SearchVisa(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? msg_status + " succesed." : msg_status + " failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;




            return data;
        }
        public PassportOutModel SetPassport(PassportOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;
            var semp_id = "";

            #region set data 

            if (data.passport_detail.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DATA_PASSPORT");
                int imaxidImg = GetMaxID("BZ_DOC_IMG");
                int imaxidProfile = GetMaxID("BZ_DOC_IMG");
                DataTable dtData = CheckAllData("BZ_DATA_PASSPORT", "");

                conn = new cls_connection_ebiz();
                if (data.passport_detail.Count > 0)
                {
                    string sqlster_delete_img = "";
                    List<passportList> dtlist = data.passport_detail;
                    for (int i = 0; i < dtlist.Count; i++)
                    {

                        //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                        string doc_status = "4";

                        Boolean bCheckDoc = false;
                        DataTable dtdoc_check = new DataTable();
                        if ((data.doc_id + "") != "personal")
                        {
                            sqlstr = @"select count(1) as xcount from  BZ_DOC_PASSPORT where doc_id = '" + data.doc_id.ToString() + "'";
                            sqlstr += " and emp_id = '" + dtlist[i].emp_id.ToString() + "' ";
                            conn = new cls_connection_ebiz();
                            if (SetDocService.conn_ExecuteData(ref dtdoc_check, sqlstr) == "")
                            {
                                if (dtdoc_check != null)
                                {
                                    if (dtdoc_check.Rows.Count > 0)
                                    {
                                        if (dtdoc_check.Rows[0]["xcount"].ToString() != "0") { bCheckDoc = true; }
                                    }
                                }
                            }
                        }

                        string id_def = "";
                        ret = "true";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true")
                            {
                                if (action_type == "update" && bCheckDoc == false)
                                {
                                    action_type = "insert";
                                    id_def = imaxid.ToString();
                                    imaxid++;
                                    goto Next_line_0;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DATA_PASSPORT
                                    (ID,DOC_ID,EMP_ID,PASSPORT_NO,PASSPORT_DATE_ISSUE,PASSPORT_DATE_EXPIRE,PASSPORT_TITLE,PASSPORT_NAME,PASSPORT_SURNAME,PASSPORT_DATE_BIRTH
                                    ,ACCEPT_TYPE,DEFAULT_TYPE,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก passport ให้ map กับ emp id
                            //sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr("", 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_no, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_date_issue, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_date_expire, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_title, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_surname, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_date_birth, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].accept_type, 300);

                            if ((data.doc_id + "") == "personal")
                            {
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].default_type, 300);
                            }
                            else
                            {
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].default_type, 300);
                            }
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";


                            //กรณีที่เป็นข้อมูลใหม่ ให้ map id ใหม่ให้กับ Img ด้วย  
                            if (data.img_list.Count > 0)
                            {
                                List<ImgList> drimg = data.img_list.Where(a => (a.id_level_1 == dtlist[i].id & a.emp_id == dtlist[i].emp_id)).ToList();
                                if (drimg.Count > 0)
                                {
                                    drimg[0].id_level_1 = imaxid.ToString();
                                }
                            }


                            id_def = imaxid.ToString();
                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DATA_PASSPORT set";

                            sqlstr += @" PASSPORT_NO = " + conn.ChkSqlStr(dtlist[i].passport_no, 300);
                            sqlstr += @" ,PASSPORT_DATE_ISSUE = " + conn.ChkSqlStr(dtlist[i].passport_date_issue, 300);
                            sqlstr += @" ,PASSPORT_DATE_EXPIRE = " + conn.ChkSqlStr(dtlist[i].passport_date_expire, 300);
                            sqlstr += @" ,PASSPORT_TITLE = " + conn.ChkSqlStr(dtlist[i].passport_title, 300);
                            sqlstr += @" ,PASSPORT_NAME = " + conn.ChkSqlStr(dtlist[i].passport_name, 300);
                            sqlstr += @" ,PASSPORT_SURNAME = " + conn.ChkSqlStr(dtlist[i].passport_surname, 300);
                            sqlstr += @" ,PASSPORT_DATE_BIRTH = " + conn.ChkSqlStr(dtlist[i].passport_date_birth, 300);
                            sqlstr += @" ,ACCEPT_TYPE = " + conn.ChkSqlStr(dtlist[i].accept_type, 300);

                            //if ((data.doc_id + "") == "personal")
                            //{
                            //if ((dtlist[i].default_action_change + "").ToString() == "true")
                            //{
                            sqlstr += @" ,DEFAULT_TYPE = " + conn.ChkSqlStr(dtlist[i].default_type, 300);
                            //}
                            //}
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก passport ให้ map กับ emp id
                            //sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            id_def = dtlist[i].id.ToString();
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DATA_PASSPORT ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก passport ให้ map กับ emp id
                            //sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            id_def = dtlist[i].id.ToString();

                        }


                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_PASSPORT ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //ไม่ต้องเก็บข้อมูล doc id เนื่องจาก passport ให้ map กับ emp id
                            //sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";
                        }

                    //เพิ่มข้อมูลใน bz_doc_passport 
                    Next_line_0:;

                        if (action_type == "insert" || action_type == "update")
                        {
                            if (dtlist[i].default_type.ToString() == "true")
                            {
                                List<EmpListOutModel> drempcheck = data.emp_list.Where(a => (a.mail_status == "true")).ToList();
                                if (drempcheck.Count > 0)
                                {
                                    semp_id = drempcheck[0].emp_id.ToString();
                                }
                            }
                            if ((data.doc_id + "") != "personal")
                            {
                                //if ((dtlist[i].default_action_change + "").ToString() == "true")
                                {
                                    sqlstr = @" delete from BZ_DOC_PASSPORT ";
                                    sqlstr += @" where ";
                                    sqlstr += @" ID = " + conn.ChkSqlStr(id_def, 300);
                                    sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                    sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                    ret = conn_ExecuteNonQuery(sqlstr, true);
                                    sqlstr_all += sqlstr + "||";

                                    sqlstr = @" insert into  BZ_DOC_PASSPORT
                                    (ID,DOC_ID,DOC_STATUS,EMP_ID,PASSPORT_NO,PASSPORT_DATE_ISSUE,PASSPORT_DATE_EXPIRE,PASSPORT_TITLE,PASSPORT_NAME,PASSPORT_SURNAME,PASSPORT_DATE_BIRTH
                                    ,ACCEPT_TYPE,DEFAULT_TYPE,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                                    sqlstr += @" " + id_def;
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(doc_status, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_no, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_date_issue, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_date_expire, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_title, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_name, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_surname, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].passport_date_birth, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].accept_type, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].default_type, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                                    sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                    sqlstr += @" ,sysdate";
                                    sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                                    sqlstr += @" )";

                                    ret = conn_ExecuteNonQuery(sqlstr, true);
                                    sqlstr_all += sqlstr + "||";

                                    sqlstr = "update BZ_DOC_PASSPORT set  ";
                                    sqlstr += @" default_type = 'false'";
                                    sqlstr += @" where ";
                                    sqlstr += @" EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                    ret = conn_ExecuteNonQuery(sqlstr, true);
                                    sqlstr_all += sqlstr + "||";

                                    sqlstr = "update BZ_DOC_PASSPORT set  ";
                                    sqlstr += @" default_type = " + conn.ChkSqlStr(dtlist[i].default_type, 300);
                                    sqlstr += @" where ";
                                    sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                    sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                    ret = conn_ExecuteNonQuery(sqlstr, true);
                                    sqlstr_all += sqlstr + "||";

                                }
                            }
                        }

                    }
                }
                if (data.img_list.Count > 0)
                {
                    ret = "true"; sqlstr = "";
                    ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";

                    //Update data new/change to bz_passport
                    ret = insertPassort(semp_id);
                }

            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchDocService swd = new SearchDocService();
                PassportModel value_load = new PassportModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new PassportOutModel();
                data = swd.SearchPassport(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        private void setDeleteImg(ref List<ImgList> data_img_list, string doc_id, string id, string emp_id)
        {
            //update status = 0 เพื่อแสดงว่าข้อมูล img ถูกลบ
            if (data_img_list.Count > 0)
            {
                List<ImgList> drimg = data_img_list.Where(a => (a.id == id & a.emp_id == emp_id)).ToList();
                if (drimg.Count > 0)
                {
                    drimg[0].status = "0";
                }
            }
        }
        public void wr(string msg)
        {
            try
            {
                string timeStampFile = DateTime.Now.ToString("yyyyMM");
                string path = System.IO.Directory.GetCurrentDirectory();
                string file_Log = @"D:\ebiz\EBiz_Webservice\Table\Log_Ebiz_2WS_" + timeStampFile + ".txt";
                string timeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ff");
                string taskComplete = (timeStamp) + " " + msg;
                using (System.IO.StreamWriter w_Log = new System.IO.StreamWriter(file_Log, true))
                {
                    w_Log.WriteLine(taskComplete);
                    w_Log.Close();
                }
            }
            catch { }
        }
        public AllowanceOutModel SetAllowance(AllowanceOutModel value)
        {
            wr("SetAllowance");

            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data 
            SearchDocService wssearch = new SearchDocService();
            DataTable dtm_exchangerate = wssearch.ref_exchangerate();

            if (data.allowance_detail.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DOC_ALLOWANCE");
                int imaxidSub = GetMaxID("BZ_DOC_ALLOWANCE_DETAIL");
                int imaxidMail = GetMaxID("BZ_DOC_ALLOWANCE_MAIL");
                int imaxidImg = GetMaxID("BZ_DOC_IMG");

                conn = new cls_connection_ebiz();
                if (data.allowance_main.Count > 0)
                {
                    List<allowanceList> dtlist = data.allowance_main;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                        string doc_status = "";
                        try
                        {
                            doc_status = dtlist[i].doc_status.ToString();
                        }
                        catch { }
                        if (doc_status == "") { doc_status = "1"; }
                        try
                        {
                            if (data.user_admin == true)
                            {
                                if (data.data_type == "submit") { doc_status = "4"; }
                                else { doc_status = "3"; }
                            }
                        }
                        catch { }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DOC_ALLOWANCE
                                    (ID,DOC_ID,DOC_STATUS,EMP_ID,GRAND_TOTAL,LUGGAGE_CLOTHING,SENDMAIL_TO_TRAVELER,REMARK 
                                    ,FILE_TRAVEL_REPORT,FILE_REPORT
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(doc_status, 20);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].grand_total, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].luggage_clothing, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].sendmail_to_traveler, "D");
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].remark, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].file_travel_report, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].file_report, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login

                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_ALLOWANCE set";

                            sqlstr += @" GRAND_TOTAL = " + conn.ChkSqlNum(dtlist[i].grand_total, "D");
                            sqlstr += @" ,LUGGAGE_CLOTHING = " + conn.ChkSqlNum(dtlist[i].luggage_clothing, "D");
                            sqlstr += @" ,SENDMAIL_TO_TRAVELER = " + conn.ChkSqlNum(dtlist[i].sendmail_to_traveler, "D");
                            sqlstr += @" ,REMARK = " + conn.ChkSqlStr(dtlist[i].remark, 300);

                            sqlstr += @" ,FILE_TRAVEL_REPORT = " + conn.ChkSqlStr(dtlist[i].file_travel_report, 300);
                            sqlstr += @" ,FILE_REPORT = " + conn.ChkSqlStr(dtlist[i].file_report, 300);

                            sqlstr += @" ,DOC_STATUS = " + conn.ChkSqlStr(doc_status, 20);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_ALLOWANCE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }
                if (data.allowance_detail.Count > 0)
                {
                    List<allowancedetailList> dtlist = data.allowance_detail;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        if (action_type == "insert" || action_type == "update")
                        {
                            string allowance_date = dtlist[i].allowance_date.ToString();//30 Oct 2019
                            string allowance_unit = dtlist[i].allowance_unit.ToString();
                            if (allowance_date != "")
                            {
                                if (allowance_unit == "") { allowance_unit = "USD"; }
                                //date_from = 05 JAN 2021
                                DataRow[] drex = dtm_exchangerate.Select("date_from='" + allowance_date + "' and currency_id ='" + allowance_unit + "' ");
                                if (drex.Length > 0)
                                {
                                    dtlist[i].exchange_rate = drex[0]["exchange_rate"].ToString();
                                }
                            }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DOC_ALLOWANCE_DETAIL
                                    (ID,DOC_ID,EMP_ID,ALLOWANCE_DATE,ALLOWANCE_DAYS,ALLOWANCE_LOW,ALLOWANCE_MID,ALLOWANCE_HIGHT
                                    ,ALLOWANCE_TOTAL,ALLOWANCE_UNIT,ALLOWANCE_HRS,ALLOWANCE_TYPE_ID,ALLOWANCE_REMARK,ALLOWANCE_EXCHANGE_RATE
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxidSub;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].allowance_date, 300);
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_days, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_low, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_mid, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_hight, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_total, "D");
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].allowance_unit, 300);
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_hrs, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_type_id, "D");
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].allowance_remark, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].exchange_rate, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 3000);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxidSub++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_ALLOWANCE_DETAIL set";

                            sqlstr += @" ALLOWANCE_DATE = " + conn.ChkSqlStr(dtlist[i].allowance_date, 300);
                            sqlstr += @" ,ALLOWANCE_DAYS = " + conn.ChkSqlNum(dtlist[i].allowance_days, "D");
                            sqlstr += @" ,ALLOWANCE_LOW = " + conn.ChkSqlNum(dtlist[i].allowance_low, "D");
                            sqlstr += @" ,ALLOWANCE_MID = " + conn.ChkSqlNum(dtlist[i].allowance_mid, "D");
                            sqlstr += @" ,ALLOWANCE_HIGHT = " + conn.ChkSqlNum(dtlist[i].allowance_hight, "D");
                            sqlstr += @" ,ALLOWANCE_TOTAL = " + conn.ChkSqlNum(dtlist[i].allowance_total, "D");
                            sqlstr += @" ,ALLOWANCE_UNIT = " + conn.ChkSqlStr(dtlist[i].allowance_unit, 300);
                            sqlstr += @" ,ALLOWANCE_HRS = " + conn.ChkSqlNum(dtlist[i].allowance_hrs, "D");
                            sqlstr += @" ,ALLOWANCE_TYPE_ID = " + conn.ChkSqlNum(dtlist[i].allowance_type_id, "D");
                            sqlstr += @" ,ALLOWANCE_REMARK = " + conn.ChkSqlStr(dtlist[i].allowance_remark, 3000);

                            //EXCHANGE_RATE,CURRENCY,AS_OF
                            sqlstr += @" ,ALLOWANCE_EXCHANGE_RATE = " + conn.ChkSqlStr(dtlist[i].exchange_rate, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_ALLOWANCE_DETAIL ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }
                    }
                }
                if (data.mail_list.Count > 0)
                {
                    List<mailselectList> dtlist = data.mail_list;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DOC_ALLOWANCE_MAIL
                                    (ID,DOC_ID,EMP_ID,MAIL_TO,MAIL_CC,MAIL_BCC,MAIL_STATUS,MAIL_REMARK,MAIL_EMP_ID
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxidMail;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].mail_to, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].mail_cc, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].mail_bcc, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].mail_status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].mail_remark, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].mail_emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxidMail++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_ALLOWANCE_MAIL set";

                            sqlstr += @" MAIL_TO = " + conn.ChkSqlStr(dtlist[i].mail_to, 300);
                            sqlstr += @" ,MAIL_CC = " + conn.ChkSqlStr(dtlist[i].mail_cc, 300);
                            sqlstr += @" ,MAIL_BCC = " + conn.ChkSqlStr(dtlist[i].mail_bcc, 300);
                            sqlstr += @" ,MAIL_STATUS = " + conn.ChkSqlStr(dtlist[i].mail_status, 4000);
                            sqlstr += @" ,MAIL_REMARK = " + conn.ChkSqlStr(dtlist[i].mail_remark, 4000);
                            sqlstr += @" ,MAIL_EMP_ID = " + conn.ChkSqlStr(dtlist[i].mail_emp_id, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_ALLOWANCE_MAIL ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }
                    }
                }
                if (data.img_list.Count > 0)
                {
                    ret = "true"; sqlstr = "";
                    ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }

            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                //018_OB/LB/OT/LT : Please submit an i-Petty Cash in Allowance - [Title_Name of traveler]
                //ใช้ SendMailAllowance

                SearchDocService swd = new SearchDocService();
                AllowanceModel value_load = new AllowanceModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new AllowanceOutModel();
                data = swd.SearchAllowance(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public ReimbursementOutModel SetReimbursement(ReimbursementOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            int imaxid = GetMaxID("BZ_DOC_REIMBURSEMENT");
            int imaxidSub = GetMaxID("BZ_DOC_REIMBURSEMENT_DETAIL");
            int imaxidMail = GetMaxID("BZ_DATA_MAIL");
            int imaxidImg = GetMaxID("BZ_DOC_IMG");

            var imaxid_def = imaxid;
            var imaxidSub_def = imaxidSub;
            var imaxidMail_def = imaxidMail;
            var imaxidImg_def = imaxidImg;
            if (data.reimbursement_main.Count > 0)
            {
                conn = new cls_connection_ebiz();

                if (data.reimbursement_main.Count > 0)
                {
                    List<reimbursementList> dtlist = data.reimbursement_main;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                        string doc_status = "";
                        List<EmpListOutModel> dtemplist = data.emp_list.Where(a => (a.mail_status == "true" && a.emp_id == dtlist[i].emp_id.ToString())).ToList();
                        for (int j = 0; j < dtemplist.Count; j++)
                        {
                            try
                            {
                                doc_status = dtemplist[j].doc_status_id.ToString();
                            }
                            catch { }
                            if (doc_status == "") { doc_status = "1"; }
                            if (data.user_admin == false) { doc_status = "2"; } else { doc_status = "3"; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DOC_REIMBURSEMENT
                                    (ID,DOC_ID,EMP_ID,DOC_STATUS,SENDMAIL_TO_TRAVELER
                                    ,FILE_TRAVEL_REPORT,FILE_REPORT
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(doc_status, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sendmail_to_traveler, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].file_travel_report, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].file_report, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_REIMBURSEMENT set";

                            sqlstr += @"  SENDMAIL_TO_TRAVELER = " + conn.ChkSqlStr(dtlist[i].sendmail_to_traveler, 300);

                            sqlstr += @" ,FILE_TRAVEL_REPORT = " + conn.ChkSqlStr(dtlist[i].file_travel_report, 300);
                            sqlstr += @" ,FILE_REPORT = " + conn.ChkSqlStr(dtlist[i].file_report, 300);
                            sqlstr += @" ,DOC_STATUS = " + conn.ChkSqlStr(doc_status, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_REIMBURSEMENT ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }
                    }
                }

                if (data.reimbursement_detail.Count > 0)
                {
                    List<reimbursementdetailList> dtlist = data.reimbursement_detail;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DOC_REIMBURSEMENT_DETAIL
                                    (ID,DOC_ID,EMP_ID,REIMBURSEMENT_DATE,DETAILS,EXCHANGE_RATE,CURRENCY,AS_OF,TOTAL,GRAND_TOTAL,REMARK
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxidSub;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].reimbursement_date, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].details, 4000);
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].exchange_rate, "D");
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].currency, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].as_of, 300);
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].total, "D");
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].grand_total, "D");
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].remark, 4000);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxidSub++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_REIMBURSEMENT_DETAIL set";

                            sqlstr += @"  REIMBURSEMENT_DATE = " + conn.ChkSqlStr(dtlist[i].reimbursement_date, 300);
                            sqlstr += @" ,DETAILS = " + conn.ChkSqlStr(dtlist[i].details, 4000);
                            sqlstr += @" ,EXCHANGE_RATE = " + conn.ChkSqlNum(dtlist[i].exchange_rate, "D");
                            sqlstr += @" ,CURRENCY = " + conn.ChkSqlStr(dtlist[i].currency, 4000);
                            sqlstr += @" ,AS_OF = " + conn.ChkSqlStr(dtlist[i].as_of, 4000);
                            sqlstr += @" ,TOTAL = " + conn.ChkSqlNum(dtlist[i].total, "D");
                            sqlstr += @" ,GRAND_TOTAL = " + conn.ChkSqlNum(dtlist[i].grand_total, "D");
                            sqlstr += @" ,REMARK = " + conn.ChkSqlStr(dtlist[i].remark, 4000);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_REIMBURSEMENT_DETAIL ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

                if (data.img_list.Count > 0)
                {
                    ret = "true"; sqlstr = "";
                    ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }

            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchDocService swd = new SearchDocService();
                ReimbursementModel value_load = new ReimbursementModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new ReimbursementOutModel();
                data = swd.SearchReimbursement(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public TravelExpenseOutModel SetTravelExpense(TravelExpenseOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            int imaxid = GetMaxID("BZ_DOC_TRAVELEXPENSE");
            int imaxidSub = GetMaxID("BZ_DOC_TRAVELEXPENSE_DETAIL");
            int imaxidMail = GetMaxID("BZ_DATA_MAIL");
            int imaxidImg = GetMaxID("BZ_DOC_IMG");

            var imaxid_def = imaxid;
            var imaxidSub_def = imaxidSub;
            var imaxidMail_def = imaxidMail;
            var imaxidImg_def = imaxidImg;

            if (doc_type == "cancelled")
            {
                ret = "True";
            }
            else
            {
                if (data.travelexpense_main.Count > 0)
                {
                    conn = new cls_connection_ebiz();

                    if (data.travelexpense_main.Count > 0)
                    {
                        List<travelexpenseList> dtlist = data.travelexpense_main;
                        for (int i = 0; i < dtlist.Count; i++)
                        {
                            ret = "true";
                            var action_type = dtlist[i].action_type.ToString();
                            if (action_type == "") { continue; }
                            else if (action_type != "delete")
                            {
                                var action_change = dtlist[i].action_change + "";
                                if (action_change.ToLower() != "true") { continue; }
                            }

                            if (action_type == "insert")
                            {
                                sqlstr = @" insert into  BZ_DOC_TRAVELEXPENSE
                                    (ID,DOC_ID,EMP_ID,SEND_TO_SAP
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                                sqlstr += @" " + imaxid;
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].send_to_sap, 300);

                                sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,sysdate";
                                sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" )";

                                imaxid++;
                            }
                            else if (action_type == "update")
                            {
                                sqlstr = @" update BZ_DOC_TRAVELEXPENSE set";

                                sqlstr += @"  SEND_TO_SAP = " + conn.ChkSqlStr(dtlist[i].send_to_sap, 300);

                                sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,UPDATE_DATE = sysdate";
                                sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            }
                            else if (action_type == "delete")
                            {
                                sqlstr = @" delete from BZ_DOC_TRAVELEXPENSE ";
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            }

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }
                        }
                    }

                    if (data.travelexpense_detail.Count > 0)
                    {
                        List<travelexpensedetailList> dtlist = data.travelexpense_detail;
                        for (int i = 0; i < dtlist.Count; i++)
                        {
                            ret = "true";
                            var action_type = dtlist[i].action_type.ToString();
                            if (action_type == "") { continue; }
                            else if (action_type != "delete")
                            {
                                var action_change = dtlist[i].action_change + "";
                                if (action_change.ToLower() != "true") { continue; }
                            }

                            if (action_type == "insert")
                            {
                                sqlstr = @" insert into  BZ_DOC_TRAVELEXPENSE_DETAIL
                                    (ID,DOC_ID,EMP_ID,EXPENSE_TYPE,DATA_DATE,STATUS,EXCHANGE_RATE,CURRENCY,AS_OF,TOTAL,GRAND_TOTAL,REMARK
                                    ,STATUS_ACTIVE
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                                sqlstr += @" " + imaxidSub;
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].expense_type, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].data_date, 300);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 4000);
                                sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].exchange_rate, "D");
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].currency, 4000);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].as_of, 300);
                                sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].total, "D");
                                sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].grand_total, "D");
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].remark, 4000);
                                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status_active, 4000);

                                sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,sysdate";
                                sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" )";

                                imaxidSub++;
                            }
                            else if (action_type == "update")
                            {
                                sqlstr = @" update BZ_DOC_TRAVELEXPENSE_DETAIL set";

                                sqlstr += @"  DATA_DATE = " + conn.ChkSqlStr(dtlist[i].data_date, 300);
                                sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 4000);
                                sqlstr += @" ,EXCHANGE_RATE = " + conn.ChkSqlNum(dtlist[i].exchange_rate, "D");
                                sqlstr += @" ,CURRENCY = " + conn.ChkSqlStr(dtlist[i].currency, 4000);
                                sqlstr += @" ,AS_OF = " + conn.ChkSqlStr(dtlist[i].as_of, 4000);
                                sqlstr += @" ,TOTAL = " + conn.ChkSqlNum(dtlist[i].total, "D");
                                sqlstr += @" ,GRAND_TOTAL = " + conn.ChkSqlNum(dtlist[i].grand_total, "D");
                                sqlstr += @" ,REMARK = " + conn.ChkSqlStr(dtlist[i].remark, 4000);
                                sqlstr += @" ,STATUS_ACTIVE = " + conn.ChkSqlStr(dtlist[i].status_active, 4000);

                                sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                sqlstr += @" ,UPDATE_DATE = sysdate";
                                sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                                //sqlstr += @" and EXPENSE_TYPE = " + conn.ChkSqlStr(dtlist[i].expense_type, 300);
                            }
                            else if (action_type == "delete")
                            {
                                sqlstr = @" delete from BZ_DOC_TRAVELEXPENSE_DETAIL ";
                                sqlstr += @" where ";
                                sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                                //sqlstr += @" and EXPENSE_TYPE = " + conn.ChkSqlStr(dtlist[i].expense_type, 300);
                            }

                            ret = conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }

                        }
                    }

                    if (data.img_list.Count > 0)
                    {
                        ret = "true"; sqlstr = "";
                        ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                        if (ret.ToLower() != "true") { goto Next_line_1; }
                    }

                Next_line_1:;

                    if (ret.ToLower() == "true")
                    {
                        ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                    }

                }
            }
            #endregion set data

            var msg_error = "";
            var msg_text = "Save data";
            var msg_text2 = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                if (doc_type == "sendtosap" || doc_type == "cancelled")
                {
                    string page_name = "travelexpense";

                    var role_type = "pmsv_admin";
                    string email_admin = "";
                    var email_traverler = "";
                    var email_apprver = "";
                    var email_requester = "";
                    var email_user_in_doc = "";
                    var email_user_display = "";
                    var emp_id_user_in_doc = "";
                    SearchDocService _swd = new SearchDocService();
                    DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin += dtemplist.Rows[i]["email"] + ";";
                    }
                    if (value.doc_id.ToString().IndexOf("T") > -1)
                    {
                        _swd = new SearchDocService();
                        dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                        for (int i = 0; i < dtemplist.Rows.Count; i++)
                        {
                            email_admin += dtemplist.Rows[i]["email"] + ";";
                        }
                    }
                    List<EmpListOutModel> emp_list = data.emp_list;
                    List<mailselectList> mail_list = new List<mailselectList>();
                    List<ImgList> img_list = data.img_list;

                    string module_name = "";
                    string sap_obj_id = "";
                    if (doc_type == "sendtosap")
                    {
                        module_name = "sendmail_to_sap";
                        msg_text = "Send to SAP";

                        //กรณีที่ส่งรายบุคคล
                        if (false)
                        {
                            //ทดสอบ update status = Send to SAP ทั้งหมดใน list ที่ส่งไป SAP ก่อน 
                            for (int i = 0; i < data.travelexpense_detail.Count; i++)
                            {
                                var action_type = data.travelexpense_detail[i].action_type.ToString();
                                if (action_type == "") { continue; }
                                else if (action_type != "delete")
                                {
                                    var action_change = data.travelexpense_detail[i].action_change + "";
                                    if (action_change.ToLower() != "true") { continue; }
                                }

                                string doc_id = data.travelexpense_detail[i].doc_id;
                                string id = data.travelexpense_detail[i].id;
                                string emp_id = data.travelexpense_detail[i].emp_id;
                                string status_sap = data.travelexpense_detail[i].status;
                                string sdate = "";
                                string edate = "";
                                string location = "";

                                List<EmpListOutModel> dremplist = data.emp_list.Where(a => ((a.emp_id == emp_id) && (a.send_to_sap == "true"))).ToList();
                                if (dremplist.Count > 0)
                                {
                                    status_sap = "6";
                                    email_user_in_doc = emp_id;
                                }
                                else { continue; }
                                if (data.travelexpense_detail[i].status_active == "true")
                                {
                                    sdate = dremplist[0].sap_from_date.ToString();
                                    edate = dremplist[0].sap_to_date.ToString();
                                    location = dremplist[0].def_location_id.ToString();
                                    try
                                    {
                                        //WS_ZTHRTEB020.SAP_ZTHRTEB020 ws_sap = new WS_ZTHRTEB020.SAP_ZTHRTEB020();
                                        //ret = ws_sap.ZTHRTEB020_DOC(doc_id, emp_id, sdate, edate, location, token_login);
                                        if (ret.ToLower() != "true")
                                        {
                                            msg_error = " SAP Error :" + ret;
                                        }
                                        else
                                        {
                                            sqlstr = @" select distinct a.doc_id, a.sap_obj_id, a.create_date
                                                 from  bz_doc_travelexpense_detail a 
                                                 where nvl(a.status,0) in ('6')  and nvl(status_active,'false') = 'true' 
                                                 and a.doc_id = '" + doc_id + "' order by a.create_date desc";
                                            DataTable dtsap = new DataTable();
                                            if (SetDocService.conn_ExecuteData(ref dtsap, sqlstr) == "")
                                            {
                                                try
                                                {
                                                    sap_obj_id = dtsap.Rows[0]["sap_obj_id"].ToString();
                                                    msg_text2 = "Object ID :" + sap_obj_id;
                                                }
                                                catch { }
                                            }

                                        }
                                    }
                                    catch (Exception ex_msg_sap) { msg_error = " SAP Error2 :" + ex_msg_sap; }
                                    break;
                                }
                            }

                            mail_list.Add(new mailselectList
                            {
                                emp_id = email_user_in_doc,
                                mail_to = email_admin,
                                mail_cc = "",
                                mail_body_in_form = "",
                                module = "sendmail_to_sap",
                            });
                        }
                        else
                        {
                            //กรณีที่เป็นการส่งรายใบงาน

                            string doc_id = "";
                            string id = "";
                            string emp_id = "";
                            string status_sap = "";
                            string sdate = "";
                            string edate = "";
                            string location = "";
                            if (data.travelexpense_detail.Count > 0)
                            {
                                for (int i = 0; i < data.travelexpense_detail.Count; i++)
                                {
                                    var action_type = data.travelexpense_detail[i].action_type.ToString();
                                    if (action_type == "") { continue; }
                                    else if (action_type != "delete")
                                    {
                                        var action_change = data.travelexpense_detail[i].action_change + "";
                                        if (action_change.ToLower() != "true") { continue; }
                                    }
                                    if (data.travelexpense_detail[i].status_active == "true")
                                    {
                                        doc_id = data.travelexpense_detail[i].doc_id;
                                        id = data.travelexpense_detail[i].id;
                                        emp_id = data.travelexpense_detail[i].emp_id;
                                        status_sap = data.travelexpense_detail[i].status;
                                        break;
                                    }
                                }
                            }
                            List<EmpListOutModel> dremplist = data.emp_list.Where(a => ((a.emp_id == emp_id) && (a.send_to_sap == "true"))).ToList();
                            if (dremplist.Count > 0)
                            {
                                email_user_in_doc = "";
                                for (int i = 0; i < data.travelexpense_detail.Count; i++)
                                {
                                    status_sap = "6";
                                    emp_id_user_in_doc = dremplist[0].emp_id.ToString();
                                    email_user_in_doc = dremplist[0].userEmail.ToString();
                                    email_user_display = dremplist[0].userDisplay.ToString();
                                    if (i == 0)
                                    {
                                        sdate = dremplist[0].sap_from_date.ToString();
                                        edate = dremplist[0].sap_to_date.ToString();
                                        location = dremplist[0].def_location_id.ToString();

                                        try
                                        {
                                            //WS_ZTHRTEB020.SAP_ZTHRTEB020 ws_sap = new WS_ZTHRTEB020.SAP_ZTHRTEB020();
                                            //ret = ws_sap.ZTHRTEB020_DOC(doc_id, "", sdate, edate, location, token_login);
                                            if (ret.ToLower() != "true")
                                            {
                                                msg_error = " SAP Error :" + ret;
                                            }
                                            else
                                            {
                                                //sqlstr = @" select distinct a.doc_id, a.sap_obj_id, a.create_date
                                                // from  bz_doc_travelexpense_detail a 
                                                // where nvl(a.status,0) in ('6')  and nvl(status_active,'false') = 'true' 
                                                // and a.doc_id = '" + doc_id + "' order by a.create_date desc";
                                                //sqlstr = @" select distinct a.doc_id, a.sap_obj_id, a.create_date
                                                // from  bz_doc_travelexpense_detail a 
                                                // where a.doc_id = '" + doc_id + "' order by a.create_date desc";

                                                sqlstr = @" select distinct a.doc_id, a.sap_obj_id, a.create_date, type_main
                                                             from BZ_DOC_TRAVELEXPENSE_SAP a 
                                                             where doc_id = '" + doc_id + "' order by to_number(a.sap_obj_id) ";
                                                DataTable dtsap = new DataTable();
                                                if (SetDocService.conn_ExecuteData(ref dtsap, sqlstr) == "")
                                                {
                                                    try
                                                    {
                                                        int isapcount = dtsap.Rows.Count;
                                                        for (int isap = 0; isap < isapcount; isap++)
                                                        {
                                                            sap_obj_id = dtsap.Rows[0]["sap_obj_id"].ToString();
                                                            if (isap == 0)
                                                            {
                                                                msg_text2 = @"Successfully sent to SAP <br>(ID : " + sap_obj_id + ")";
                                                                break;
                                                                //if (isapcount > 1) { msg_text2 += "("; }
                                                            }
                                                            else
                                                            {
                                                                //if (isap == 1) { msg_text2 += ","; }
                                                                //msg_text2 += sap_obj_id;
                                                            }
                                                        }
                                                        if (isapcount > 1) { msg_text2 += ")"; }
                                                    }
                                                    catch { }
                                                }
                                            }
                                        }
                                        catch (Exception ex_msg_sap) { msg_error = " SAP Error2 :" + ex_msg_sap; }
                                        break;
                                    }
                                }
                            }

                        }

                        email_user_in_doc = "";
                        mail_list.Add(new mailselectList
                        {
                            module = "sendmail_to_sap",
                            mail_to = email_admin,
                            mail_body_in_form = "",
                            mail_cc = email_user_in_doc,
                            emp_id = emp_id_user_in_doc,
                            mail_status = "true",
                            action_change = "true",
                        });

                        ret = "";
                        SendEmailService swmail = new SendEmailService();
                        ret = swmail.SendMailInPage(ref mail_list, emp_list, img_list, data.doc_id, page_name, module_name);

                    }
                    else if (doc_type == "cancelled")
                    {
                        module_name = "tripcancelled";
                        #region cancelled
                        var doc_id = data.doc_id;
                        var cancel_reason = "";
                        try
                        {
                            cancel_reason = value.travelexpense_main[0].remark.ToString();
                        }
                        catch { }

                        #region กณีที่ไม่มีข้อมูลให้เพิ่มใหม่ เพื่อใช้ในการตรวจสอบ tracing
                        sqlstr = @"select count(1) as xcount from BZ_DOC_TRAVELEXPENSE where doc_id = '" + doc_id + "'";
                        dt = new DataTable();
                        conn = new cls_connection_ebiz();
                        if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["xcount"].ToString() == "0")
                                {
                                    if (data.travelexpense_main.Count > 0)
                                    {
                                        List<travelexpenseList> dtlist = data.travelexpense_main;
                                        for (int i = 0; i < dtlist.Count; i++)
                                        {
                                            ret = "true";

                                            sqlstr = @" insert into  BZ_DOC_TRAVELEXPENSE (ID,DOC_ID,EMP_ID,SEND_TO_SAP,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                                            sqlstr += @" " + imaxid;
                                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].send_to_sap, 300);

                                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                                            sqlstr += @" ,sysdate";
                                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                                            sqlstr += @" )";

                                            imaxid++;
                                            ret = conn_ExecuteNonQuery(sqlstr, false);

                                        }
                                    }
                                }
                            }
                        }
                        #endregion กณีที่ไม่มีข้อมูลให้เพิ่มใหม่ เพื่อใช้ในการตรวจสอบ tracing

                        sqlstr = " update BZ_DOC_TRAVELEXPENSE set STATUS_TRIP_CANCELLED = 'true' , REMARK =  " + ChkSqlStr(cancel_reason, 4000) + "  where doc_id = '" + doc_id + "' ";
                        ret = conn_ExecuteNonQuery(sqlstr, false);

                        if (ret.ToLower() != "true")
                        {
                            msg_error = ret + " --> query error :" + sqlstr;
                        }
                        else
                        {
                            #region send mail   
                            dtemplist = _swd.refsearch_empapprer_list(doc_id);
                            for (int i = 0; i < dtemplist.Rows.Count; i++)
                            {
                                email_apprver += dtemplist.Rows[i]["email"] + ";";
                            }

                            dtemplist = _swd.refsearch_emprequester_list(doc_id);
                            for (int i = 0; i < dtemplist.Rows.Count; i++)
                            {
                                email_requester += dtemplist.Rows[i]["email"] + ";";
                            }

                            var emp_id_select = "";
                            List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                            if (drempcheck.Count > 0)
                            {
                                for (int i = 0; i < drempcheck.Count; i++)
                                {
                                    if (emp_id_select != "") { emp_id_select += ";"; }
                                    emp_id_select = drempcheck[i].emp_id;
                                    if (email_traverler != "") { email_traverler += ";"; }
                                    email_traverler += drempcheck[i].userEmail;
                                }
                            }

                            mail_list.Add(new mailselectList
                            {
                                emp_id = emp_id_select,
                                mail_to = email_traverler + email_apprver + email_requester,
                                mail_cc = email_admin,
                                mail_body_in_form = " Reason of Cancelled : " + cancel_reason,
                                module = "tripcancelled",
                            });

                            ret = "";
                            SendEmailService swmail = new SendEmailService();
                            ret = swmail.SendMailInPage(ref mail_list, emp_list, img_list, data.doc_id, page_name, module_name);

                            #endregion send mail 
                        }
                        #endregion cancelled

                    }
                }
                SearchDocService swd = new SearchDocService();
                TravelExpenseModel value_load = new TravelExpenseModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new TravelExpenseOutModel();
                data = swd.SearchTravelExpense(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? (msg_text + " succesed." + msg_text2) : (msg_text + " data failed.");
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public TravelInsuranceOutModel SetTravelInsurance(TravelInsuranceOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;
            Boolean action_change_emp_select = false;//ใช้เพื่อเช็คในการส่ง mail   


            #region set data  
            if (data.travelInsurance_detail.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DOC_INSURANCE");
                int imaxidImg = GetMaxID("BZ_DOC_IMG");

                conn = new cls_connection_ebiz();

                if (data.travelInsurance_detail.Count > 0)
                {
                    List<travelinsuranceList> dtlist = data.travelInsurance_detail;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        //ใช้เพื่อเช็คในการส่ง mail 
                        try
                        {
                            List<EmpListOutModel> drempcheck = data.emp_list.Where(a => ((a.mail_status == "true") && (a.emp_id == dtlist[i].emp_id + ""))).ToList();
                            if (drempcheck.Count > 0)
                            {
                                if (dtlist[i].certificates_no.ToString() != "") { action_change_emp_select = true; }
                            }
                        }
                        catch { }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into BZ_DOC_INSURANCE
                                    (ID,DOC_ID,EMP_ID,INS_EMP_ID,INS_EMP_NAME,INS_EMP_ORG,INS_EMP_PASSPORT,INS_EMP_AGE
                                    ,NAME_BENEFICIARY,RELATIONSHIP,PERIOD_INS_DEST,PERIOD_INS_FROM,PERIOD_INS_TO,DESTINATION,DATE_EXPIRE
                                    ,DURATION,BILLING_CHARGE,CERTIFICATES_NO
                                    ,INS_EMP_ADDRESS,INS_EMP_OCCUPATION,INS_EMP_TEL,INS_EMP_FAX,INS_PLAN
                                    ,AGENT_TYPE,BROKER_TYPE,TRAVEL_AGENT_TYPE,INSURANCE_COMPANY
                                    ,INS_BROKER,CERTIFICATES_TOTAL,REMARK,SORT_BY
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_org, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_passport, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_age, 300);


                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name_beneficiary, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].relationship, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].period_ins_dest, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].period_ins_from, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].period_ins_to, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].destination, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].date_expire, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].duration, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].billing_charge, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].certificates_no, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_address, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_occupation, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_tel, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_emp_fax, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_plan, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].agent_type, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].broker_type, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].travel_agent_type, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].insurance_company, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].ins_broker, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].certificates_total, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].remark, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            //กรณีที่เป็นข้อมูลใหม่ ให้ map id ใหม่ให้กับ Img ด้วย  
                            if (data.img_list.Count > 0)
                            {
                                List<ImgList> drimg = data.img_list.Where(a => (a.id_level_1 == dtlist[i].id & a.emp_id == dtlist[i].emp_id)).ToList();
                                if (drimg.Count > 0)
                                {
                                    drimg[0].id_level_1 = imaxid.ToString();
                                }
                            }
                            //กรณีที่เป็นข้อมูลใหม่ ให้ map id ใหม่ให้กับ Img ด้วย  
                            if (data.img_list.Count > 0)
                            {
                                List<ImgList> drimg = data.img_list.Where(a => (a.id_level_1 == dtlist[i].id & a.emp_id == dtlist[i].emp_id)).ToList();
                                if (drimg.Count > 0)
                                {
                                    drimg[0].id_level_1 = imaxid.ToString();
                                }
                            }

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_INSURANCE set";

                            sqlstr += @"  INS_EMP_ID = " + conn.ChkSqlStr(dtlist[i].ins_emp_id, 300);
                            sqlstr += @" ,INS_EMP_NAME = " + conn.ChkSqlStr(dtlist[i].ins_emp_name, 300);
                            sqlstr += @" ,INS_EMP_ORG = " + conn.ChkSqlStr(dtlist[i].ins_emp_org, 300);
                            sqlstr += @" ,INS_EMP_PASSPORT = " + conn.ChkSqlStr(dtlist[i].ins_emp_passport, 300);
                            sqlstr += @" ,INS_EMP_AGE = " + conn.ChkSqlStr(dtlist[i].ins_emp_age, 300);
                            sqlstr += @" ,NAME_BENEFICIARY = " + conn.ChkSqlStr(dtlist[i].name_beneficiary, 300);
                            sqlstr += @" ,RELATIONSHIP = " + conn.ChkSqlStr(dtlist[i].relationship, 300);
                            sqlstr += @" ,PERIOD_INS_DEST = " + conn.ChkSqlStr(dtlist[i].period_ins_dest, 300);
                            sqlstr += @" ,PERIOD_INS_FROM = " + conn.ChkSqlStr(dtlist[i].period_ins_from, 300);
                            sqlstr += @" ,PERIOD_INS_TO = " + conn.ChkSqlStr(dtlist[i].period_ins_to, 300);
                            sqlstr += @" ,DESTINATION = " + conn.ChkSqlStr(dtlist[i].destination, 300);
                            sqlstr += @" ,DATE_EXPIRE = " + conn.ChkSqlStr(dtlist[i].date_expire, 300);
                            sqlstr += @" ,DURATION = " + conn.ChkSqlStr(dtlist[i].duration, 300);
                            sqlstr += @" ,BILLING_CHARGE = " + conn.ChkSqlStr(dtlist[i].billing_charge, 300);
                            sqlstr += @" ,CERTIFICATES_NO = " + conn.ChkSqlStr(dtlist[i].certificates_no, 300);

                            sqlstr += @" ,INS_EMP_ADDRESS = " + conn.ChkSqlStr(dtlist[i].ins_emp_address, 4000);
                            sqlstr += @" ,INS_EMP_OCCUPATION = " + conn.ChkSqlStr(dtlist[i].ins_emp_occupation, 300);
                            sqlstr += @" ,INS_EMP_TEL = " + conn.ChkSqlStr(dtlist[i].ins_emp_tel, 300);
                            sqlstr += @" ,INS_EMP_FAX = " + conn.ChkSqlStr(dtlist[i].ins_emp_fax, 300);
                            sqlstr += @" ,INS_PLAN = " + conn.ChkSqlStr(dtlist[i].ins_plan, 300);

                            sqlstr += @" ,AGENT_TYPE = " + conn.ChkSqlStr(dtlist[i].agent_type, 300);
                            sqlstr += @" ,BROKER_TYPE = " + conn.ChkSqlStr(dtlist[i].broker_type, 300);
                            sqlstr += @" ,TRAVEL_AGENT_TYPE = " + conn.ChkSqlStr(dtlist[i].travel_agent_type, 300);
                            sqlstr += @" ,INSURANCE_COMPANY = " + conn.ChkSqlStr(dtlist[i].insurance_company, 300);

                            sqlstr += @" ,INS_BROKER = " + conn.ChkSqlStr(dtlist[i].ins_broker, 300);
                            sqlstr += @" ,CERTIFICATES_TOTAL = " + conn.ChkSqlStr(dtlist[i].certificates_total, 300);

                            sqlstr += @" ,REMARK = " + conn.ChkSqlStr(dtlist[i].remark, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_INSURANCE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

                if (data.img_list.Count > 0)
                {
                    ret = "true"; sqlstr = "";
                    ret = SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }

            }

            //update doc status //เนื่องจาก update ทีละ emp id อยุ่แล้ว 
            if (data.travelInsurance_detail.Count > 0)
            {
                List<EmpListOutModel> dtlist = data.emp_list.Where(a => (a.mail_status == "true")).ToList();
                for (int i = 0; i < dtlist.Count; i++)
                {
                    //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                    string doc_status = "";
                    try
                    {
                        doc_status = dtlist[i].doc_status_id.ToString();
                    }
                    catch { }
                    if (doc_status == "") { doc_status = "1"; }
                    if (data.user_admin == false) { doc_status = "2"; } else { doc_status = "3"; }

                    Boolean bCheckStatus = false;
                    sqlstr = @" select  (select count(1) as total from BZ_DOC_INSURANCE where doc_id ='" + data.doc_id + "' and emp_id = '" + dtlist[i].emp_id + "'  )";
                    sqlstr += @" - (select count(1) as total from BZ_DOC_INSURANCE where doc_id ='" + data.doc_id + "' and emp_id = '" + dtlist[i].emp_id + "' and (certificates_no is not null ) )";
                    sqlstr += @"  as xcount from dual ";
                    DataTable dtref = new DataTable();
                    if (SetDocService.conn_ExecuteData(ref dtref, sqlstr) == "")
                    {
                        if (dtref.Rows.Count > 0)
                        {
                            if (dtref.Rows[0]["xcount"].ToString() == "0") { doc_status = "4"; bCheckStatus = true; }
                        }
                    }

                    if (bCheckStatus == true)
                    {
                        doc_status = "4";
                    }

                    sqlstr = @"update BZ_DOC_INSURANCE set DOC_STATUS = '" + doc_status + "' where doc_id ='" + data.doc_id + "' and emp_id = '" + dtlist[i].emp_id + "' ";
                    ret = conn_ExecuteNonQuery(sqlstr, false);

                    dtlist[i].doc_status_id = doc_status;
                }
            }

            #endregion set data

            var msg_error = "";
            var msg_status = "Save data";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                //--> 022_OB/LB/OT/LT : Travel Insurance Certificate has been completed - [Title_Name of traveler]
                //doc_type = "save" และ status page = complated
                if (doc_type == "save" && action_change_emp_select == true) { doc_type = "sendmail_to_been_completed"; }
                if (doc_type == "sendmail_to_insurance" || doc_type == "sendmail_to_traveler" || doc_type == "sendmail_to_been_completed")
                {
                    var page_name = "travelinsurance";
                    var module_name = doc_type;
                    var email_admin = "";
                    var email_user_in_doc = "";
                    var email_ins_broker = "";
                    var mail_cc_active = "";
                    var mail_body_in_form = "";
                    var file_outbound_name = "";
                    var mail_to_display = "";
                    var email_attachments = "";

                    var role_type = "pmsv_admin";
                    var emp_id_user_in_doc = "";
                    var module = "";

                    List<EmpListOutModel> emp_list = data.emp_list;
                    List<mailselectList> mail_list = data.mail_list;

                    SearchDocService _swd = new SearchDocService();
                    DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin += dtemplist.Rows[i]["email"] + ";";
                    }
                    if (value.doc_id.ToString().IndexOf("T") > -1)
                    {
                        _swd = new SearchDocService();
                        dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                        for (int i = 0; i < dtemplist.Rows.Count; i++)
                        {
                            email_admin += dtemplist.Rows[i]["email"] + ";";
                        }
                    }
                    //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
                    mail_cc_active = sqlEmpUserMail(value.token_login);

                    List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck.Count > 0)
                    {
                        emp_id_user_in_doc = drempcheck[0].emp_id.ToString();
                        email_user_in_doc = drempcheck[0].userEmail.ToString();
                    }

                    List<travelinsuranceList> drlistcheck = data.travelInsurance_detail.Where(a => (a.emp_id == emp_id_user_in_doc)).ToList();
                    if (drlistcheck.Count > 0) { file_outbound_name = drlistcheck[0].file_outbound_name.ToString(); }

                    if (doc_type == "sendmail_to_insurance")
                    {
                        msg_status = "Send to Broker";
                        module = "Sendmail to Broker";
                        //send mail 
                        //to : บริษัทฯประกัน???  travelInsurance_detail.ins_broker 
                        //insurance Form ที่เป็นหน้า higlith สีเหลือง น้ำเงิน
                        //*** file ยังไม่แน่ใจว่าสามารถ gen doc tp word ได้หรือไม่ถ้าไม่ได้ ให้ส่ง url 
                        //*** เขียน service แยก ??
                        sqlstr = @"select a.id,a.doc_id, a.ins_broker ,mb.email ,mb.name as ins_broker_name
                                    from  bz_doc_insurance a
                                    left join bz_master_insurance_company mb on a.ins_broker = mb.id
                                    where to_number(a.id) =  (select max(to_number(a2.id))as id from bz_doc_insurance a2 where a.doc_id = a2.doc_id and a.emp_id = a2.emp_id)
                                    and a.doc_id = '" + data.doc_id + "' and a.emp_id = '" + emp_id_user_in_doc + "' ";
                        DataTable dtref = new DataTable();
                        if (conn_ExecuteData(ref dtref, sqlstr) == "")
                        {
                            if (dtref.Rows.Count > 0)
                            {
                                for (int k = 0; k < dtref.Rows.Count; k++)
                                {
                                    email_ins_broker += dtref.Rows[k]["email"].ToString() + ";";
                                }
                                if (dtref.Rows.Count > 1) { mail_to_display = "All"; } else { mail_to_display = dtref.Rows[0]["ins_broker_name"].ToString(); }

                            }
                            else
                            {
                                mail_body_in_form = "ไม่พบข้อมูล Broker ";
                            }
                        }

                        string _FolderMailAttachments = System.Configuration.ConfigurationManager.AppSettings["FilePathServerApp"].ToString();
                        string mail_attachments = _FolderMailAttachments + @"temp\" + file_outbound_name;
                        mail_body_in_form += " <br>คำขอเอาประกันภัยการเดินทางต่างประเทศ : " + file_outbound_name + "(ถ้าแก้ไขเรื่องแนบไฟล์ให้เอาออก)";

                        mail_list = new List<mailselectList>();
                        mail_list.Add(new mailselectList
                        {
                            module = module,
                            mail_to = email_ins_broker,
                            mail_to_display = mail_to_display,
                            mail_body_in_form = mail_body_in_form,
                            mail_cc = email_admin,
                            mail_attachments = mail_attachments,
                            emp_id = emp_id_user_in_doc,
                            mail_status = "true",
                            action_change = "true",
                        });

                        //data.img_list
                    }
                    else if (doc_type == "sendmail_to_traveler")
                    {
                        msg_status = "Send to Traveler";
                        module = "Sendmail to Traveler";
                        //send mail 
                        //to : emp_list.mail_status = "true"
                        //cc : user int mail_list & emp ที่ emp_list.mail_status = "true"

                        mail_list = new List<mailselectList>();
                        mail_list.Add(new mailselectList
                        {
                            module = module,
                            mail_to = email_user_in_doc,
                            mail_body_in_form = mail_body_in_form,
                            mail_cc = email_admin,
                            emp_id = emp_id_user_in_doc,
                            mail_status = "true",
                            action_change = "true",
                        });
                    }
                    else if (doc_type == "sendmail_to_been_completed")
                    {
                        msg_status = "Send to Traveler";
                        module = "Sendmail to Traveler";

                        try
                        {
                            List<ImgList> drimgcheck = data.img_list.Where(a => (a.emp_id == emp_id_user_in_doc) && a.action_type != "delete").ToList();
                            for (int i = 0; i < drimgcheck.Count; i++)
                            {
                                if (email_attachments != "") { email_attachments += ";"; }

                                if ((drimgcheck[i].fullname + "" + "") == "")
                                {
                                    email_attachments += drimgcheck[i].path + "" + drimgcheck[i].filename;
                                }
                                else
                                {
                                    email_attachments += drimgcheck[i].fullname;
                                }
                            }
                        }
                        catch { }

                        //send mail 
                        //to : emp_list.mail_status = "true"
                        //cc : admin 
                        mail_list = new List<mailselectList>();
                        mail_list.Add(new mailselectList
                        {
                            module = module,
                            mail_to = email_user_in_doc,
                            mail_body_in_form = mail_body_in_form,
                            mail_cc = email_admin,
                            mail_attachments = email_attachments,
                            emp_id = emp_id_user_in_doc,
                            mail_status = "true",
                            action_change = "true",
                        });
                    }



                    
                    logService.logModel mLog = new logService.logModel();

                    mLog.module = "SetTravelInsurance" + value.doc_id;
                    mLog.tevent = email_attachments;
                    mLog.ref_id = 0;
                    //mLog.data_log = JsonSerializer.Serialize(value);
                    logService.insertLog(mLog);

                    ret = "";
                    SendEmailService swmail = new SendEmailService();
                    ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id, page_name, module_name);
                    if (ret.ToLower() != "true")
                    {
                        msg_error = ret;
                    }
                }

                SearchDocService swd = new SearchDocService();
                TravelInsuranceModel value_load = new TravelInsuranceModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new TravelInsuranceOutModel();
                data = swd.SearchTravelInsurance(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? msg_status + " succesed." : msg_status + " failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }

        private string SetISOSRecord(List<isosList> dtlist, string emp_user_active, string token_login)
        {
            int imaxid = GetMaxIDYear("BZ_DOC_ISOS_RECORD");

            for (int i = 0; i < dtlist.Count; i++)
            {
                if (dtlist[i].send_mail_type.ToString() != "0") { continue; }

                sqlstr = @" insert into BZ_DOC_ISOS_RECORD
                                    (ID,YEAR,DOC_ID,EMP_ID,ISOS_TYPE_OF_TRAVEL,ISOS_EMP_ID,ISOS_EMP_TITLE,ISOS_EMP_NAME,ISOS_EMP_SURNAME,ISOS_EMP_SECTION,ISOS_EMP_DEPARTMENT,ISOS_EMP_FUNCTION,SEND_MAIL_TYPE,INSURANCE_COMPANY_ID
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                sqlstr += @" " + imaxid;
                sqlstr += @" ,to_char(sysdate,'rrrr')";
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_type_of_travel, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_emp_id, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_emp_title, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_emp_name, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_emp_surname, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_emp_section, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_emp_department, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].isos_emp_function, 300);

                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].send_mail_type, 300);
                sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].insurance_company_id, 300);

                sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                sqlstr += @" ,sysdate";
                sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                sqlstr += @" )";

                ret = conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";

                if (ret.ToLower() != "true") { goto Next_line_1; }

                dtlist[i].id = imaxid.ToString();
                imaxid++;

            }
        Next_line_1:

            if (ret.ToLower() == "true")
            {
                ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
            }

            return ret;
        }

        public ISOSOutModel SetISOSMain(ISOSOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;
            var doc_id = data.doc_id;
            var page_name = "isos";
            //emp_user_active = get_emp_user();

            string msg_error = "";
            Boolean type_save = true;
            SetContentHTML(token_login, doc_id, emp_user_active, page_name, value.html_content, value.img_list, ref msg_error, ref ret, ref type_save, ref sqlstr_all);

            if (type_save == true)
            {
                ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
            }

            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchDocService swd = new SearchDocService();
                ISOSModel value_load = new ISOSModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new ISOSOutModel();
                data = swd.SearchISOS(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }

        public void SetContentHTML(
            string token_login, string doc_id, string emp_user_active
            , string page_name, string html_content
            , List<ImgList> img_list
            , ref string msg_error
            , ref string ret
            , ref Boolean type_save
            , ref string sqlstr_all)
        {
            #region set data  
            if (true)
            {
                //genarate file to server
                string content_path = "";
                string content_name = "data.txt";
                //ข้อมูลเป็นเเบบ Base64 เก็บไว้ใน db ไม่ได้ จึงเก็บไว้ในรูปแบบไฟล์ .txt ที่ \\10.224.43.14\EBiz2\EBiz_Webservice\ExportFile\OB20120006\isos\data.txt
                try
                {
                    //C:\inetpub\wwwroot\ebiz_service\ebiz.webservice\ebiz.webservice\DocumentFile\ISOS
                    string file_doc = doc_id;
                    string file_page = page_name.ToLower();
                    string file_emp = "";
                    string _Server_path = System.Configuration.ConfigurationManager.AppSettings["ServerPath_Service"].ToString();
                    string _Folder = "/DocumentFile/" + file_page + "/";
                    string _Path = "";
                        //System.Web.HttpContext.Current.Server.MapPath("~" + _Folder);

                    string data_isos = html_content;
                    string file_Log = _Path + content_name;
                    //??? ไม่ create folder ให้ auto ไมวะ
                    using (StreamWriter w_File_Data = new StreamWriter(file_Log, false))
                    {
                        w_File_Data.WriteLine(data_isos);
                        w_File_Data.Close();
                    }
                    content_path = _Folder;
                }
                catch { }

                int imaxid = GetMaxID("BZ_DATA_CONTENT");
                int imaxidImg = GetMaxID("BZ_DOC_IMG");

                conn = new cls_connection_ebiz();

                if (true)
                {
                    ret = "true";
                    sqlstr = "delete from BZ_DATA_CONTENT where page_name ='" + page_name + "'";
                    ret = conn_ExecuteNonQuery(sqlstr, true);
                    sqlstr_all += sqlstr + "||";
                    if (ret.ToLower() != "true") { goto Next_line_1; }

                    sqlstr = "insert into BZ_DATA_CONTENT (ID,PAGE_NAME,CONTENT_PATH,CONTENT_NAME,REMARK,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values (";
                    sqlstr += @" " + imaxid;
                    sqlstr += @" ," + conn.ChkSqlStr(page_name, 300);
                    sqlstr += @" ," + conn.ChkSqlStr(content_path, 300);
                    sqlstr += @" ," + conn.ChkSqlStr(content_name, 300);
                    sqlstr += @" ," + conn.ChkSqlStr("", 300);

                    sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                    sqlstr += @" ,sysdate";
                    sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                    sqlstr += @" )";

                    ret = conn_ExecuteNonQuery(sqlstr, true);
                    sqlstr_all += sqlstr + "||";
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }
                if (img_list.Count > 0)
                {
                    for (int i = 0; i < img_list.Count; i++)
                    {
                        img_list[i].pagename = "isos";
                    }
                    ret = "true"; sqlstr = "";
                    ret = SetImgList(img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    if (type_save == true)
                    {
                        ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                    }
                }

            }
            #endregion set data

            msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
        }


        public TransportationOutModel SetTransportation(TransportationOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;
            var doc_id = data.doc_id;
            var page_name = "transportation";
            //emp_user_active = get_emp_user();

            string msg_error = "";
            Boolean type_save = true;
            SetContentHTML(token_login, doc_id, emp_user_active, page_name, value.html_content, value.img_list, ref msg_error, ref ret, ref type_save, ref sqlstr_all);

            if (type_save == true)
            {
                ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";

                #region Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed

                string doc_status = "";
                var emp_id_select = "";
                List<EmpListOutModel> drempcheck = data.emp_list.Where(a => (a.mail_status == "true")).ToList();
                if (drempcheck.Count > 0)
                {
                    emp_id_select = drempcheck[0].emp_id;

                    try
                    {
                        doc_status = drempcheck[0].doc_status_id.ToString();
                    }
                    catch { }
                }

                //doc_type = submit --> send mail กรณีที่ส่ง mail ได้ ต้องมี Car ID เรียบร้อยแล้ว
                if (doc_type == "submit") { doc_status = "4"; }

                sqlstr = @"delete from BZ_DATA_CONTENT_EMP where doc_id = '" + doc_id.ToString() + "' and emp_id = '" + emp_id_select + "' ";
                ret = conn_ExecuteNonQuery(sqlstr, false);

                sqlstr = @"insert into  BZ_DATA_CONTENT_EMP ( doc_id, emp_id, doc_status) values ( '" + doc_id.ToString() + "', '" + emp_id_select + "', '" + doc_status + "' )";
                ret = conn_ExecuteNonQuery(sqlstr, false);

                #endregion Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed


            }

            msg_error = "";
            var msg_status = "Save data";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                //doc_type = submit
                if (doc_type == "submit")
                {
                    //ส่ง mail พร้อมแนบไฟล์/ลิ้ง ส่งให้พนักงาน อาจจะมีพนักงงานหลายคน  
                    string url_personal_car_document = data.url_personal_car_document;


                    msg_status = "Submit Data";
                    var module_name = doc_type;
                    var email_admin = "";
                    var email_user_in_doc = "";
                    var mail_cc_active = "";
                    var role_type = "pmsv_admin";
                    var emp_id_user_in_doc = "";

                    List<EmpListOutModel> emp_list = data.emp_list;
                    List<mailselectList> mail_list = new List<mailselectList>();

                    SearchDocService _swd = new SearchDocService();
                    DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin += dtemplist.Rows[i]["email"] + ";";
                    }
                    if (value.doc_id.ToString().IndexOf("T") > -1)
                    {
                        _swd = new SearchDocService();
                        dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                        for (int i = 0; i < dtemplist.Rows.Count; i++)
                        {
                            email_admin += dtemplist.Rows[i]["email"] + ";";
                        }
                    }
                    //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
                    mail_cc_active = sqlEmpUserMail(value.token_login);

                    List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck.Count > 0)
                    {
                        for (int i = 0; i < drempcheck.Count; i++)
                        {
                            //if (email_user_in_doc != "") { email_user_in_doc += ";"; }
                            //if (emp_id_user_in_doc != "") { emp_id_user_in_doc += ";"; }
                            emp_id_user_in_doc += drempcheck[i].emp_id.ToString() + ";";
                            email_user_in_doc += drempcheck[i].userEmail.ToString() + ";";
                        }
                    }
                    // to travler  cc admin  ตาม mail_status = 'true'
                    mail_list.Add(new mailselectList
                    {
                        module = "Transportation",
                        mail_to = email_user_in_doc,
                        mail_cc = email_admin,
                        mail_attachments = url_personal_car_document,
                        mail_body_in_form = "",
                        mail_status = "true",
                        action_change = "true",
                        emp_id = emp_id_user_in_doc,
                    });

                    ret = "";
                    SendEmailService swmail = new SendEmailService();
                    ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id, page_name, module_name);
                    if (ret.ToLower() != "true")
                    {
                        msg_error = ret;
                    }
                }

                SearchDocService swd = new SearchDocService();
                TransportationModel value_load = new TransportationModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new TransportationOutModel();
                data = swd.SearchTransportation(value_load);

            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? msg_status + " succesed." : msg_status + " failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }

        public FeedbackOutModel SetFeedback(FeedbackOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;
            var doc_id = value.doc_id;

            //emp_user_active = get_emp_user();

            #region set data  
            if (data.feedback_detail.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DOC_FEEDBACK");

                conn = new cls_connection_ebiz();

                if (data.feedback_detail.Count > 0)
                {
                    #region Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                    string doc_status = "";
                    var emp_id_select = "";
                    List<EmpListOutModel> drempcheck = data.emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck.Count > 0)
                    {
                        emp_id_select = drempcheck[0].emp_id;
                        try
                        {
                            doc_status = drempcheck[0].doc_status_id.ToString();
                        }
                        catch { }
                        if (value.user_admin == true)
                        {
                            //doc_status = "3";
                        }
                        else
                        {
                            doc_status = "2";
                            //doc_type = submit
                            if (doc_type == "submit")
                            {
                                doc_status = "4";
                            }
                        }

                    }
                    #endregion Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed


                    List<feedbackList> dtlist = data.feedback_detail;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into BZ_DOC_FEEDBACK
                                    (ID,DOC_ID,EMP_ID,DOC_STATUS
                                    ,FEEDBACK_TYPE_ID,FEEDBACK_LIST_ID,FEEDBACK_QUESTION_ID,QUESTION_OTHER,NO,QUESTION,DESCRIPTION,ANSWER
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(doc_status, 20);


                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].feedback_type_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].feedback_list_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].feedback_question_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].question_other, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].no, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].question, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].description, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].answer, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DOC_FEEDBACK set";

                            sqlstr += @" FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].feedback_type_id, 300);
                            sqlstr += @" ,FEEDBACK_LIST_ID = " + conn.ChkSqlStr(dtlist[i].feedback_list_id, 300);
                            sqlstr += @" ,FEEDBACK_QUESTION_ID = " + conn.ChkSqlStr(dtlist[i].feedback_question_id, 300);
                            sqlstr += @" ,QUESTION_OTHER = " + conn.ChkSqlStr(dtlist[i].question_other, 300);

                            sqlstr += @" ,NO = " + conn.ChkSqlStr(dtlist[i].no, 300);
                            sqlstr += @" ,QUESTION = " + conn.ChkSqlStr(dtlist[i].question, 300);
                            sqlstr += @" ,DESCRIPTION = " + conn.ChkSqlStr(dtlist[i].description, 300);
                            sqlstr += @" ,ANSWER = " + conn.ChkSqlStr(dtlist[i].answer, 300);
                            sqlstr += @" ,DOC_STATUS = " + conn.ChkSqlStr(doc_status, 20);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DOC_FEEDBACK ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(dtlist[i].doc_id, 300);
                            sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";


                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        sqlstr = @" update BZ_DOC_FEEDBACK set";
                        sqlstr += @" DOC_STATUS = " + conn.ChkSqlStr(doc_status, 20);
                        sqlstr += @" where ";
                        sqlstr += @" DOC_ID = " + conn.ChkSqlStr(doc_id, 300);
                        sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(emp_id_select, 300);
                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";
                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }

                }


            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }
            }
            #endregion set data


            var msg_error = "";
            var msg_status = "Save data";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                //doc_type = submit
                if (doc_type == "submit")
                {
                    msg_status = "Submit Data";
                    var page_name = "feedback";
                    var module_name = doc_type;
                    var email_admin = "";
                    var email_user_in_doc = "";
                    var mail_cc_active = "";
                    var role_type = "pmsv_admin";
                    var emp_id_user_in_doc = "";

                    List<EmpListOutModel> emp_list = data.emp_list;
                    List<mailselectList> mail_list = new List<mailselectList>();
                    List<ImgList> img_list = new List<ImgList>();

                    SearchDocService _swd = new SearchDocService();
                    DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin += dtemplist.Rows[i]["email"] + ";";
                    }
                    if (value.doc_id.ToString().IndexOf("T") > -1)
                    {
                        _swd = new SearchDocService();
                        dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                        for (int i = 0; i < dtemplist.Rows.Count; i++)
                        {
                            email_admin += dtemplist.Rows[i]["email"] + ";";
                        }
                    }
                    //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
                    mail_cc_active = sqlEmpUserMail(value.token_login);

                    List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck.Count > 0)
                    {
                        emp_id_user_in_doc = drempcheck[0].emp_id.ToString();
                        email_user_in_doc = drempcheck[0].userEmail.ToString();
                    }
                    // to travler  cc admin  ตาม mail_status = 'true'
                    mail_list.Add(new mailselectList
                    {
                        module = "Feedback",
                        mail_to = email_user_in_doc,
                        mail_cc = email_admin,
                        emp_id = emp_id_user_in_doc,
                        mail_status = "true",
                        action_change = "true",
                        mail_body_in_form = "",
                    });


                    ret = "";
                    SendEmailService swmail = new SendEmailService();
                    ret = swmail.SendMailInPage(ref mail_list, emp_list, img_list, data.doc_id, page_name, module_name);
                    if (ret.ToLower() != "true")
                    {
                        msg_error = ret;
                    }
                }

                SearchDocService swd = new SearchDocService();
                FeedbackModel value_load = new FeedbackModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new FeedbackOutModel();
                data = swd.SearchFeedback(value_load);

            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? msg_status + " succesed." : msg_status + " failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }

        public PortalOutModel SetPortal(PortalOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete เนื่องจากมีข้อมูลเพียงชุดเดียว
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            if (true)
            {
                //img_header,img_personal_profile,img_banner_1,img_banner_2,img_banner_3,img_practice_areas,get_in_touch
                string action_change_imgname = data.action_change_imgname;
                if (action_change_imgname != "")
                {
                    conn = new cls_connection_ebiz();

                    sqlstr = @" update BZ_DOC_PORTAL set CREATE_BY = CREATE_BY ";
                    if (action_change_imgname.ToLower() == "img_header")
                    {
                        sqlstr += @" ,IMG_HEADER = " + conn.ChkSqlStr(data.img_list[0].img_header, 4000);
                    }
                    else if (action_change_imgname.ToLower() == "img_personal_profile")
                    {
                        sqlstr += @" ,IMG_PERSONAL_PROFILE = " + conn.ChkSqlStr(data.img_list[0].img_personal_profile, 4000);
                    }
                    else if (action_change_imgname.ToLower() == "img_banner_1")
                    {
                        sqlstr += @" ,IMG_BANNER_1 = " + conn.ChkSqlStr(data.img_list[0].img_banner_1, 4000);
                        sqlstr += @" ,URL_BANNER_1 = " + conn.ChkSqlStr(data.img_list[0].url_banner_1, 4000);
                    }
                    else if (action_change_imgname.ToLower() == "img_banner_2")
                    {
                        sqlstr += @" ,IMG_BANNER_2 = " + conn.ChkSqlStr(data.img_list[0].img_banner_2, 4000);
                        sqlstr += @" ,URL_BANNER_2 = " + conn.ChkSqlStr(data.img_list[0].url_banner_2, 4000);
                    }
                    else if (action_change_imgname.ToLower() == "img_banner_3")
                    {
                        sqlstr += @" ,IMG_BANNER_3 = " + conn.ChkSqlStr(data.img_list[0].img_banner_3, 4000);
                        sqlstr += @" ,URL_BANNER_3 = " + conn.ChkSqlStr(data.img_list[0].url_banner_3, 4000);
                    }
                    else if (action_change_imgname.ToLower() == "img_practice_areas")
                    {
                        sqlstr += @" ,IMG_PRACTICE_AREAS = " + conn.ChkSqlStr(data.img_list[0].img_practice_areas, 4000);
                    }
                    else if (action_change_imgname.ToLower() == "title")
                    {
                        sqlstr += @" ,TEXT_TITLE = " + conn.ChkSqlStr(data.text_title, 4000);
                        sqlstr += @" ,TEXT_DESC = " + conn.ChkSqlStr(data.text_desc, 4000);
                    }
                    else if (action_change_imgname.ToLower() == "get_in_touch")
                    {
                        sqlstr += @" ,TEXT_CONTACT_TITLE = " + conn.ChkSqlStr(data.text_contact_title, 4000);
                        sqlstr += @" ,TEXT_CONTACT_EMAIL = " + conn.ChkSqlStr(data.text_contact_email, 4000);
                        sqlstr += @" ,TEXT_CONTACT_TEL = " + conn.ChkSqlStr(data.text_contact_tel, 4000);
                    }

                    sqlstr += @" ,URL_EMPLOYEE_PRIVACY_CENTER = " + conn.ChkSqlStr(data.url_employee_privacy_center, 4000);


                    sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                    sqlstr += @" ,UPDATE_DATE = sysdate";
                    sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                    sqlstr += @" where ";
                    sqlstr += @" ID = " + conn.ChkSqlStr(data.id, 300);

                    ret = conn_ExecuteNonQuery(sqlstr, false);

                }

            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }

        public ManageRoleOutModel SetManageRole(ManageRoleOutModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            if (data.admin_list.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DATA_MANAGE");

                conn = new cls_connection_ebiz();

                value.after_add_user = new List<userNewList>();
                if (data.admin_list.Count > 0)
                {
                    List<roleList> drlistCheck = data.admin_list.Where(a => ((a.emp_id == "") && a.action_type != "delete")).ToList();
                    for (int i = 0; i < drlistCheck.Count; i++)
                    {
                        string userid = drlistCheck[i].username.ToString();
                        string emp_id = "";
                        string _msg = "";
                        //userid ค้นหาใน ad 
                        emp_id = AddUser(userid, ref _msg);
                        if (emp_id != "")
                        {
                            drlistCheck[i].emp_id = emp_id;
                            drlistCheck[i].username = userid.ToString().ToUpper();
                        }
                        else
                        {
                            //value.after_user.opt1 
                            value.after_add_user.Add(new userNewList
                            {
                                username = userid,
                                status = "false",
                                remark = _msg,
                            });
                        }
                    }

                    List<roleList> dtlist = data.admin_list;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }

                            if (dtlist[i].emp_id == "") { continue; }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into BZ_DATA_MANAGE
                                    (ID,EMP_ID,USER_ID
                                    ,SUPER_ADMIN,PMSV_ADMIN,PMDV_ADMIN,CONTACT_ADMIN,SORT_BY,STATUS
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].username, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].super_admin, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].pmsv_admin, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].pmdv_admin, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].contact_admin, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DATA_MANAGE set";

                            sqlstr += @" USER_ID = " + conn.ChkSqlStr(dtlist[i].username, 300);
                            sqlstr += @" ,EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);
                            sqlstr += @" ,SUPER_ADMIN = " + conn.ChkSqlStr(dtlist[i].super_admin, 300);
                            sqlstr += @" ,PMSV_ADMIN = " + conn.ChkSqlStr(dtlist[i].pmsv_admin, 300);
                            sqlstr += @" ,PMDV_ADMIN = " + conn.ChkSqlStr(dtlist[i].pmdv_admin, 300);
                            sqlstr += @" ,CONTACT_ADMIN = " + conn.ChkSqlStr(dtlist[i].contact_admin, 300);

                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //sqlstr += @" and USER_ID = " + conn.ChkSqlStr(dtlist[i].username, 300);

                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DATA_MANAGE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            //sqlstr += @" and USER_ID = " + conn.ChkSqlStr(dtlist[i].username, 300);
                        }

                        ret = conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;

                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }
                // เพิ่มเส้น update ข้อมูล bz_users.role_id   
                sqlstr = "update bz_users a set a.role_id = (select case when  b.SUPER_ADMIN = 'true'  then 1 else 0 end  from bz_data_manage b where a.userid = b.user_id) ";
                ret = conn_ExecuteNonQuery(sqlstr, false);

            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchDocService swd = new SearchDocService();
                ManageRoleModel value_load = new ManageRoleModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new ManageRoleOutModel();
                data = swd.SearchManageRole(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            try
            {
                //value.after_add_user 
                data.after_add_user = value.after_add_user;
            }
            catch { }

            return data;
        }


        public KHCodeOutModel SetKHCode(KHCodeOutModel value)
        {
            //กรณีนี้ข้อมูลไม่มี type ที่เป็น insert และ delete
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data 
            Boolean user_admin = false;
            string user_id = "";
            string user_role = "";
            sqlEmpRole(token_login, ref user_id, ref user_role, ref user_admin, "");
            emp_user_active = user_id;

            if (data.khcode_list.Count > 0)
            {
                int imaxid = GetMaxID("BZ_DATA_KH_CODE");

                conn = new cls_connection_ebiz();

                List<khcodeList> dtlist = data.khcode_list;
                for (int i = 0; i < dtlist.Count; i++)
                {
                    ret = "true";
                    var id_def = "";
                    var action_type = dtlist[i].action_type.ToString();
                    if (action_type == "") { continue; }
                    else if (action_type != "delete")
                    {
                        var action_change = dtlist[i].action_change + "";
                        if (action_change.ToLower() != "true") { continue; }
                    }

                    if (action_type == "insert")
                    {
                        sqlstr = @" insert into BZ_DATA_KH_CODE
                                    (ID,EMP_ID,USER_ID,OVERSEA_CODE,LOCAL_CODE,STATUS
                                    ,DATA_FOR_SAP
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                        sqlstr += @" " + imaxid;
                        sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                        sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].user_id, 300);
                        sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].oversea_code, 300);
                        sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].local_code, 300);
                        sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);

                        sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].data_for_sap, 300); //sap = 1, ebiz = 0

                        sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                        sqlstr += @" ,sysdate";
                        sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                        sqlstr += @" )";


                        id_def = imaxid.ToString();
                        imaxid++;
                    }
                    else if (action_type == "update")
                    {
                        sqlstr = @" update BZ_DATA_KH_CODE set";

                        sqlstr += @" OVERSEA_CODE = " + conn.ChkSqlStr(dtlist[i].oversea_code, 300);
                        sqlstr += @" ,LOCAL_CODE = " + conn.ChkSqlStr(dtlist[i].local_code, 300);
                        sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);

                        sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                        sqlstr += @" ,UPDATE_DATE = sysdate";
                        sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                        sqlstr += @" where ";
                        sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                        id_def = dtlist[i].id.ToString();

                    }
                    else if (action_type == "delete")
                    {
                        sqlstr = @" delete from BZ_DATA_KH_CODE ";
                        sqlstr += @" where ";
                        sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(dtlist[i].emp_id, 300);

                        id_def = dtlist[i].id.ToString();
                    }

                    ret = conn_ExecuteNonQuery(sqlstr, true);
                    sqlstr_all += sqlstr + "||";

                    if (ret.ToLower() != "true") { goto Next_line_1; }

                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
                }

            }

            #endregion set data

            var msg_error = "";
            var msg_status = "Save data";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                if (doc_type == "submit")
                {
                    msg_status = "Submit Data";
                }
                else
                {
                    msg_status = "Save";
                }

                SearchDocService swd = new SearchDocService();
                KHCodeModel value_load = new KHCodeModel();
                value_load.token_login = data.token_login;
                data = new KHCodeOutModel();
                data = swd.SearchKHCode(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? msg_status + " succesed." : msg_status + " failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;




            return data;
        }

        #endregion set data in page

        #region set send mail in page 

        public AllowanceOutModel SendMailAllowance(AllowanceOutModel value)
        {
            var msg_error = "";
            var doc_type = value.data_type;
            var page_name = "allowance";
            var data = value;

            var email_admin = "";
            var role_type = "pmsv_admin";
            SearchDocService _swd = new SearchDocService();
            DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
            for (int i = 0; i < dtemplist.Rows.Count; i++)
            {
                email_admin += dtemplist.Rows[i]["email"] + ";";
            }
            if (value.doc_id.ToString().IndexOf("T") > -1)
            {
                _swd = new SearchDocService();
                dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                for (int i = 0; i < dtemplist.Rows.Count; i++)
                {
                    email_admin += dtemplist.Rows[i]["email"] + ";";
                }
            }
            List<mailselectList> mail_list = data.mail_list;
            List<EmpListOutModel> emp_list = data.emp_list;
            List<ImgList> img_list = data.img_list;
            try
            {
                msg_error += "line:1";
                //mail_list.mail_attachments = full path
                List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                msg_error += "line:11";
                if (drempcheck.Count > 0)
                {
                    //"http://TBKC-DAPPS-05.thaioil.localnet/ebiz_ws/ExportFile/OB20090026/allowance/00001109/Allowance_Payment_Test.xlsx"
                    var emp_id_select = drempcheck[0].emp_id.ToString();
                    if (emp_id_select != "")
                    {
                        mail_list = new List<mailselectList>();
                        mail_list = data.mail_list.Where(a => (a.emp_id == emp_id_select)).ToList();

                        msg_error += "line:12";
                        List<allowanceList> drcheck = value.allowance_main.Where(a => (a.emp_id == emp_id_select)).ToList();
                        if (drcheck.Count > 0)
                        {
                            //mail_list[0].mail_attachments = drcheck[0].file_report + ";" + drcheck[0].file_travel_report; 
                            for (int i = 0; i < mail_list.Count; i++)
                            {
                                mail_list[i].mail_attachments = drcheck[0].file_report + ";" + drcheck[0].file_travel_report;
                            }
                        }
                        //Allowance Payment Form

                        //เพิ่ม to admin ,cc : traverler 
                        var email_user_in_doc = drempcheck[0].userEmail.ToString();
                        var email_user_display = drempcheck[0].userDisplay.ToString();
                        mail_list[0].mail_to += email_admin;
                        mail_list[0].mail_to_display = email_user_display;
                        mail_list[0].mail_cc += email_user_in_doc;
                    }

                }

                msg_error += "line:21";
                ret = "";
                SendEmailService swmail = new SendEmailService();
                ret = swmail.SendMailInPage(ref mail_list, emp_list, img_list, data.doc_id, page_name, "");
                if (ret == "true")
                {
                    msg_error += "line:22";
                    if (value.user_admin == true)
                    {
                        //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                        List<EmpListOutModel> drempcheck2 = emp_list.Where(a => (a.mail_status == "true")).ToList();
                        if (drempcheck2.Count > 0)
                        {
                            sqlstr = @" update bz_doc_allowance set doc_status = '4' 
                                        where doc_id = '" + value.doc_id.ToString() + "' and emp_id = '" + drempcheck2[0].emp_id.ToString() + "' ";

                            ret = conn_ExecuteNonQuery(sqlstr, false);
                        }

                    }
                }
                msg_error += "line:31";
                msg_error += ret;
            }
            catch (Exception ex) { msg_error += ex.Message.ToString(); ret = "false"; }
            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public ReimbursementOutModel SendMailReimbursement(ReimbursementOutModel value)
        {
            //send mail 
            //to : user int mail_list
            //cc : user int mail_list
            //subject : E-Biz : Reimbursement
            //body :  
            //body url : 
            //attachments :  

            var msg_error = "";
            var doc_type = value.data_type;
            var page_name = "reimbursement";
            var data = value;

            var email_admin = "";
            var role_type = "pmsv_admin";
            SearchDocService _swd = new SearchDocService();
            DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
            for (int i = 0; i < dtemplist.Rows.Count; i++)
            {
                email_admin += dtemplist.Rows[i]["email"] + ";";
            }
            if (value.doc_id.ToString().IndexOf("T") > -1)
            {
                _swd = new SearchDocService();
                dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                for (int i = 0; i < dtemplist.Rows.Count; i++)
                {
                    email_admin += dtemplist.Rows[i]["email"] + ";";
                }
            }

            List<mailselectList> mail_list = data.mail_list;
            List<EmpListOutModel> emp_list = data.emp_list;
            List<ImgList> img_list = data.img_list;

            try
            {
                msg_error += "line:1";
                //mail_list.mail_attachments = full path
                List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                msg_error += "line:11";
                if (drempcheck.Count > 0)
                {
                    //"http://TBKC-DAPPS-05.thaioil.localnet/ebiz_ws/ExportFile/OB20090026/allowance/00001109/Allowance_Payment_Test.xlsx"
                    var emp_id_select = drempcheck[0].emp_id.ToString();
                    if (emp_id_select != "")
                    {
                        //ตัดให้เหลือรายการเดียว 
                        mail_list = new List<mailselectList>();
                        mail_list = data.mail_list.Where(a => (a.emp_id == emp_id_select)).ToList();

                        msg_error += "line:12";
                        List<reimbursementList> drcheck = value.reimbursement_main.Where(a => (a.emp_id == emp_id_select)).ToList();
                        if (drcheck.Count > 0)
                        {
                            msg_error += "line:13";
                            List<mailselectList> drmailcheck = mail_list.Where(a => (a.emp_id == emp_id_select)).ToList();
                            drmailcheck[0].mail_attachments = drcheck[0].file_report + ";" + drcheck[0].file_travel_report;
                            msg_error += "line:14";


                            //เพิ่ม to admin ,cc : traverler 
                            var email_user_in_doc = drempcheck[0].userEmail.ToString();
                            var email_user_display = drempcheck[0].userDisplay.ToString();
                            mail_list[0].mail_to += email_admin;
                            mail_list[0].mail_to_display = email_user_display;
                            mail_list[0].mail_cc += email_user_in_doc;

                        }
                    }
                    //เพิ่มไฟล์แนบจากหน้า web
                    if (img_list.Count > 0)
                    {
                        List<mailselectList> drmailcheck = mail_list.Where(a => (a.emp_id == emp_id_select)).ToList();
                        List<ImgList> drimg_list = data.img_list.Where(a => (a.emp_id == emp_id_select)).ToList();
                        for (int i = 0; i < drimg_list.Count; i++)
                        {
                            if (drimg_list[i].fullname.ToString() != "")
                            {
                                if (drmailcheck[0].mail_attachments != "") { drmailcheck[0].mail_attachments += ";"; }
                                drmailcheck[0].mail_attachments += drimg_list[i].fullname;
                            }
                        }
                    }
                }


                msg_error += "line:21";
                ret = "";
                SendEmailService swmail = new SendEmailService();
                ret = swmail.SendMailInPage(ref mail_list, emp_list, img_list, data.doc_id, page_name, "");

                msg_error += "line:31";
                msg_error = "";

                if (ret == "true")
                {
                    if (value.user_admin == true)
                    {
                        //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                        List<EmpListOutModel> drempcheck2 = emp_list.Where(a => (a.mail_status == "true")).ToList();
                        if (drempcheck2.Count > 0)
                        {
                            sqlstr = @" update BZ_DOC_REIMBURSEMENT set doc_status = '4' 
                                        where doc_id = '" + value.doc_id.ToString() + "' and emp_id = '" + drempcheck2[0].emp_id.ToString() + "' ";
                            ret = conn_ExecuteNonQuery(sqlstr, false);
                        }
                    }
                }

            }
            catch (Exception ex) { msg_error = ex.Message.ToString(); ret = "false"; }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public TravelInsuranceOutModel SendMailTravelinsurance(TravelInsuranceOutModel value)
        {
            //send mail 
            //to : user int mail_list
            //cc : user int mail_list
            //subject : E-Biz : Send mail to Traveler 
            //body :  
            //body url : 
            //attachments :  

            var msg_error = "";
            var doc_type = value.data_type;
            var page_name = "travelinsurance";
            var data = value;

            List<mailselectList> mail_list = data.mail_list;
            List<EmpListOutModel> emp_list = data.emp_list;
            List<ImgList> img_list = data.img_list;

            ret = "";
            SendEmailService swmail = new SendEmailService();
            ret = swmail.SendMailInPage(ref mail_list, emp_list, img_list, data.doc_id, page_name, "");

            if (ret == "true")
            {
                if (value.user_admin == true)
                {
                    //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                    List<EmpListOutModel> drempcheck2 = emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck2.Count > 0)
                    {
                        sqlstr = @" update bz_doc_insurance set doc_status = '3' 
                                        where doc_id = '" + value.doc_id.ToString() + "' and emp_id = '" + drempcheck2[0].emp_id.ToString() + "' ";
                        ret = conn_ExecuteNonQuery(sqlstr, false);
                    }
                }
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public TravelInsuranceOutModel SendMailTravelinsuranceClaim(TravelInsuranceOutModel value)
        {
            //send mail 
            //to : user int mail_list
            //cc : user int mail_list
            //subject : E-Biz : Send mail to Traveler 
            //body :  
            //body url : 
            //attachments :  

            var msg_error = "";
            var doc_type = value.data_type;
            var page_name = "travelinsurance";
            var data = value;

            List<mailselectList> mail_list = data.mail_list;
            List<EmpListOutModel> emp_list = data.emp_list;
            List<ImgList> img_list = data.img_list;
            //ตรวจสอบว่ามีการ active เพื่อส่ง mail หรือไม่จาก mail_status = 'true'
            List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
            if (drempcheck.Count > 0)
            {
                string emp_id_def = drempcheck[0].emp_id.ToString() + "";
                string email_emp_def = "";
                string email_admin_def = "";
                mail_list = new List<mailselectList>();
                for (int i = 0; i < drempcheck.Count; i++)
                {
                    email_emp_def += drempcheck[i].userEmail.ToString() + ";";
                }
                var role_type = "pmsv_admin";
                SearchDocService _swd = new SearchDocService();
                DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                for (int i = 0; i < dtemplist.Rows.Count; i++)
                {
                    email_admin_def += dtemplist.Rows[i]["email"] + ";";
                }
                if (value.doc_id.ToString().IndexOf("T") > -1)
                {
                    _swd = new SearchDocService();
                    dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin_def += dtemplist.Rows[i]["email"] + ";";
                    }
                }
                mail_list.Add(new mailselectList
                {
                    module = "claim form requisition",
                    emp_id = emp_id_def,
                    mail_to = email_emp_def,
                    mail_body_in_form = "",
                    mail_cc = email_admin_def,
                    mail_status = "true",
                    action_change = "true",
                }); ;
            }

            ret = "";
            SendEmailService swmail = new SendEmailService();
            ret = swmail.SendMailInPage(ref mail_list, emp_list, img_list, data.doc_id, page_name, "claim form requisition");

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public TransportationOutModel SendMailTransportation(TransportationOutModel value)
        {
            //ส่ง mail พร้อมแนบไฟล์/ลิ้ง ส่งให้พนักงาน อาจจะมีพนักงงานหลายคน 
            SearchDocService swd = new SearchDocService();
            string url_personal_car_document = swd.refdata_TransportationURL(value.token_login, value.doc_id, "");

            var doc_type = value.data_type;
            var ret = "";
            var data = value;

            int icount_emp = 0;
            string slist_emp_mail_to = "";
            string s_mail_to_emp_name = "";

            string s_mail_to = ""; // emp_list.mail_staus = 'true'
            string s_mail_cc = "";
            string s_subject = "";
            string s_mail_body = "";
            string s_mail_attachments = "";

            for (int i = 0; i < data.emp_list.Count; i++)
            {
                if ((data.emp_list[i].mail_status + "").ToString() == "true")
                {
                    icount_emp += 1;
                    slist_emp_mail_to += data.emp_list[i].userEmail + ";";

                    s_mail_to_emp_name = data.emp_list[i].userDisplay;
                }
            }
            var role_type = "pmsv_admin";
            var email_admin = "";
            List<EmpListOutModel> emp_list = data.emp_list;
            List<mailselectList> mail_list = new List<mailselectList>();

            SearchDocService _swd = new SearchDocService();
            DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
            for (int i = 0; i < dtemplist.Rows.Count; i++)
            {
                email_admin += dtemplist.Rows[i]["email"] + ";";
            }
            if (value.doc_id.ToString().IndexOf("T") > -1)
            {
                _swd = new SearchDocService();
                dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                for (int i = 0; i < dtemplist.Rows.Count; i++)
                {
                    email_admin += dtemplist.Rows[i]["email"] + ";";
                }
            }

            s_mail_to = slist_emp_mail_to + email_admin;
            s_mail_cc = "";

            s_subject = "E-Biz : Transportation Form";
            s_mail_body = @"Dear " + s_mail_to_emp_name + "";
            if (icount_emp > 1)
            {
                s_mail_body = "Dear All";
            }
            s_mail_body += @"";
            s_mail_body += @"Regards, 
                                <br>Thiti Noydee (Yo)
                                <br>System Administration Officer
                                <br>
                                <br>Tel : 038-359000  Ext 20104";


            SendEmailService sm = new SendEmailService();
            sm.send_mail(s_mail_to, s_mail_cc, s_subject, s_mail_body, s_mail_attachments);

            for (int i = 0; i < data.emp_list.Count; i++)
            {
                if ((data.emp_list[i].mail_status + "").ToString() == "true")
                {
                    if (ret.ToLower().Replace("true", "") == "")
                    {
                        data.emp_list[i].mail_remark = "Send mail Success.";
                    }
                    else
                    {
                        data.emp_list[i].mail_remark = "Send mail Error." + ret;
                    }
                }
            }

            return data;
        }
        public VisaOutModel SendMailVisa(VisaOutModel value)
        {
            //ส่ง mail พร้อมแนบไฟล์/ลิ้ง ส่งให้พนักงาน อาจจะมีพนักงงานหลายคน 

            var msg_error = "";
            var doc_type = value.data_type;
            var page_name = "visa";
            var data = value;


            List<mailselectList> mail_list;//= data.mail_list;
            List<EmpListOutModel> emp_list = data.emp_list;
            List<ImgList> img_list = data.img_list;

            List<visaList> visa_detail = data.visa_detail;
            string country_in_doc = "";
            string email_admin = "";
            string email_user_in_doc = "";
            string emp_id_user_in_doc = "";
            string email_user_display = "";
            try
            {
                //Auwat 20210630 1200 แก้ไขเนื่องจาก font แจ้งมาว่าไม่ได้ใช้งานเส้นนี้ในการส่ง mail Visa Requisition จะใช้ SendMailVisa
                var role_type = "pmsv_admin";
                SearchDocService _swd = new SearchDocService();
                DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
                for (int i = 0; i < dtemplist.Rows.Count; i++)
                {
                    email_admin += dtemplist.Rows[i]["email"] + ";";
                }
                if (value.doc_id.ToString().IndexOf("T") > -1)
                {
                    _swd = new SearchDocService();
                    dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                    for (int i = 0; i < dtemplist.Rows.Count; i++)
                    {
                        email_admin += dtemplist.Rows[i]["email"] + ";";
                    }
                }
                var emp_id_select = "";
                List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                if (drempcheck.Count > 0)
                {
                    emp_id_select = drempcheck[0].emp_id;
                    emp_id_user_in_doc = drempcheck[0].emp_id;
                    email_user_in_doc = drempcheck[0].userEmail;
                    email_user_display = drempcheck[0].userDisplay;
                }

                List<MasterCountryModel> drcountrycheck = data.country_doc.Where(a => (a.action_change == "true")).ToList();
                if (drcountrycheck.Count > 0)
                {
                    country_in_doc = "";
                    for (int i = 0; i < drcountrycheck.Count; i++)
                    {
                        if (country_in_doc != "") { country_in_doc += ","; }
                        country_in_doc += drcountrycheck[i].country_id;
                    }
                }

            }
            catch { }

            string module_name = "sendmail_visa_requisition";
            mail_list = new List<mailselectList>();

            if (module_name == "sendmail_visa_requisition")
            {
                List<mailselectList> mail_list_def = new List<mailselectList>();

                mail_list_def = data.mail_list.Where(a => ((a.emp_id.ToLower() == emp_id_user_in_doc))).ToList();
                if (mail_list_def.Count > 0)
                {
                    mail_list.Add(new mailselectList
                    {
                        module = "VISA Requisition",
                        mail_to = mail_list_def[0].mail_to + email_user_in_doc,
                        mail_to_display = email_user_display,
                        mail_cc = mail_list_def[0].mail_cc + email_admin,
                        mail_body_in_form = "",
                        mail_status = "true",
                        action_change = "true",
                        emp_id = emp_id_user_in_doc,
                        country_in_doc = country_in_doc,
                    });
                }
            }

            ret = "";
            SendEmailService swmail = new SendEmailService();
            ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id, page_name, module_name);


            if (ret == "")
            {
                if (value.user_admin == true)
                {
                    //Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
                    List<EmpListOutModel> drempcheck2 = emp_list.Where(a => (a.mail_status == "true")).ToList();
                    if (drempcheck2.Count > 0)
                    {
                        string emp_id_select = drempcheck2[0].emp_id;
                        string visa_card_id = "";
                        List<visaList> dtlist = data.visa_detail.Where(a => (a.emp_id == emp_id_select && a.visa_active_in_doc == "true")).ToList();
                        for (int i = 0; i < dtlist.Count; i++)
                        {
                            if (visa_card_id != "") { visa_card_id += ","; }
                            visa_card_id += "'" + dtlist[i].visa_card_id.ToString() + "'";
                        }
                        sqlstr = @" update bz_doc_visa set doc_status = '3' 
                                        where emp_id = '" + drempcheck2[0].emp_id.ToString() + "' and doc_id = '" + value.doc_id + "' ";
                        //sqlstr = @" update bz_data_visa set doc_status = '3' 
                        //                where emp_id = '" + drempcheck2[0].emp_id.ToString() + "' and visa_card_id in (" + visa_card_id + ") ";
                        ret = conn_ExecuteNonQuery(sqlstr, false);
                    }
                }
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }

        public ISOSOutModel SendMailISOS(ISOSOutModel value)
        {
            //ส่ง mail  ส่งให้พนักงานที่อยู่ในใบงาน 
            //mail isos : to all user ในใบงานนั้นๆ, cc pmsv group
            var data = value;
            var msg_error = "";
            var page_name = "isos";
            var module_name = value.data_type;
            var email_admin = "";
            var email_user_in_doc = "";
            var mail_cc_active = "";
            var role_type = "pmsv_admin";
            var emp_id_user_in_doc = "";
            var email_user_display = "";

            List<EmpListOutModel> emp_list = data.emp_list;
            List<mailselectList> mail_list = new List<mailselectList>();

            SearchDocService _swd = new SearchDocService();
            DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
            for (int i = 0; i < dtemplist.Rows.Count; i++)
            {
                email_admin += dtemplist.Rows[i]["email"] + ";";
            }
            if (value.doc_id.ToString().IndexOf("T") > -1)
            {
                _swd = new SearchDocService();
                dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                for (int i = 0; i < dtemplist.Rows.Count; i++)
                {
                    email_admin += dtemplist.Rows[i]["email"] + ";";
                }
            }
            //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
            mail_cc_active = sqlEmpUserMail(value.token_login);

            List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
            if (drempcheck.Count > 0)
            {
                for (int i = 0; i < drempcheck.Count; i++)
                {
                    emp_id_user_in_doc += drempcheck[i].emp_id.ToString() + ";";//ไม่ได้ใช้งาน แต่เก็บไว้ให้ครบ
                    email_user_in_doc += drempcheck[i].userEmail.ToString() + ";";
                    email_user_display += drempcheck[i].userDisplay.ToString() + ";";
                }
            }
            ///???
            module_name = "sendmail_isos_to_traveler";
            mail_list.Add(new mailselectList
            {
                module = "ISOS",
                mail_to = email_user_in_doc,
                mail_to_display = email_user_display,
                mail_body_in_form = "",
                mail_cc = email_admin,
                emp_id = emp_id_user_in_doc,
                mail_status = "true",
                action_change = "true",
            });

            ret = "";
            SendEmailService swmail = new SendEmailService();
            ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id, page_name, module_name);
            if (ret.ToLower() != "true")
            {
                msg_error = ret;
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public ISOSOutModel SendMailISOSRecord(ISOSOutModel value)
        {
            //ส่ง mail  ส่งให้พนักงานที่อยู่ในใบงาน 
            //mail isos : to broker ในใบงานนั้นๆ, cc pmsv group
            var data = value;
            var msg_error = "";
            var page_name = "isos";
            var module_name = value.data_type;
            var email_admin = "";
            var email_user_in_doc = "";
            var mail_cc_active = "";
            var role_type = "pmsv_admin";
            var emp_id_user_in_doc = "";
            var email_broker = "";
            var email_broker_id = "";
            var email_broker_name = "";

            for (int i = 0; i < value.m_broker.Count; i++)
            {
                if (value.m_broker[i].status.ToString() == "true")
                {
                    email_broker += value.m_broker[i].email + ";";

                    if (email_broker_id != "") { email_broker_id += ","; }
                    email_broker_id += value.m_broker[i].id;
                    if (email_broker_name != "") { email_broker_name += ","; }
                    email_broker_name += value.m_broker[i].name;
                }
            }

            List<EmpListOutModel> emp_list = data.emp_list;
            List<mailselectList> mail_list = new List<mailselectList>();

            SearchDocService _swd = new SearchDocService();
            DataTable dtemplist = _swd.refsearch_emprole_list(role_type);
            for (int i = 0; i < dtemplist.Rows.Count; i++)
            {
                email_admin += dtemplist.Rows[i]["email"] + ";";
            }

            //auwat 20221003 1424 น้องเมล์ เมล์ที่ออกมา ไม่ต้อง CC พนักงานก็ได้ค่ะ แต่ช่วย CC  PMDV team ด้วยค่ะ
            //if (value.doc_id.ToString().IndexOf("T") > -1)
            {
                _swd = new SearchDocService();
                dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
                for (int i = 0; i < dtemplist.Rows.Count; i++)
                {
                    email_admin += dtemplist.Rows[i]["email"] + ";";
                }
            }
            //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
            mail_cc_active = sqlEmpUserMail(value.token_login);

            List<EmpListOutModel> drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
            //List<EmpListOutModel> drempcheck = emp_list.Where(a => ("true" == "true")).ToList();
            if (drempcheck.Count > 0)
            {
                sqlstr = "";
                for (int i = 0; i < drempcheck.Count; i++)
                {
                    emp_id_user_in_doc += drempcheck[i].emp_id.ToString() + ";";
                    email_user_in_doc += drempcheck[i].userEmail.ToString() + ";";
                    //ตรวจสอบว่าข้อมูลของพนักงาน เคยส่งไปให้ Broker แล้วหรือไม่ ถ้ามีไม่ต้องส่งไปใหม่ 
                    //broker เดียว เเต่ E-Mail อาจจะมีหลายเมลล์ 
                    //travel type ต่างกันต้อง run number ชุดเดียวค่ะ คุ้มครองพนักงานบิรษัท 
                    if (i > 0) { sqlstr += " union "; }
                    sqlstr += @" select a.emp_id, nvl(b.send_mail_type,0) as send_mail_type 
                                    , u.entitle as title, u.enfirstname as name, u.enlastname as surname, u.sections as section, u.department, u.function 
                                    from(select '" + drempcheck[i].emp_id.ToString() + "' as emp_id from dual)a  ";
                    sqlstr += @" left join bz_users u on a.emp_id = u.employeeid
                                     left join bz_doc_isos_record b on a.emp_id = b.emp_id and b.year = to_char(sysdate,'rrrr')
                                     where nvl(b.send_mail_type,0) = 0";

                }
                if (sqlstr != "")
                {
                    dt = new DataTable();
                    conn = new cls_connection_ebiz();
                    if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                    {
                        ret = "true";
                        if (dt.Rows.Count > 0)
                        {
                            //เพิ่มข้อมูลใหม่ 
                            string emp_user_active = "";
                            string token_login = value.token_login.ToString();
                            string doc_id = value.doc_id.ToString();

                            string type_of_travel = "Business Trip";
                            if (doc_id.ToLower().IndexOf("ot") > -1 || doc_id.ToLower().IndexOf("lt") > -1)
                            {
                                type_of_travel = "Training Trip";
                            }

                            List<isosList> dtlist = new List<isosList>();
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                if (dt.Rows[j]["send_mail_type"].ToString() == "1") { continue; }
                                isosList deflist = new isosList();
                                deflist.id = (j + 1).ToString();
                                deflist.doc_id = value.doc_id;
                                deflist.emp_id = dt.Rows[j]["emp_id"].ToString();
                                deflist.send_mail_type = "0";

                                deflist.isos_type_of_travel = type_of_travel;
                                deflist.isos_emp_id = dt.Rows[j]["emp_id"].ToString();
                                deflist.isos_emp_title = dt.Rows[j]["title"].ToString();
                                deflist.isos_emp_name = dt.Rows[j]["name"].ToString();
                                deflist.isos_emp_surname = dt.Rows[j]["surname"].ToString();
                                deflist.isos_emp_section = dt.Rows[j]["section"].ToString();
                                deflist.isos_emp_department = dt.Rows[j]["department"].ToString();
                                deflist.isos_emp_function = dt.Rows[j]["function"].ToString();

                                deflist.insurance_company_id = email_broker_id;
                                dtlist.Add(deflist);
                            }
                            if (dtlist.Count > 0)
                            {
                                ret = SetISOSRecord(dtlist, emp_user_active, token_login);

                                if (ret.ToLower() == "true")
                                {
                                    module_name = "sendmail_isos_to_broker";
                                    mail_list = new List<mailselectList>();
                                    mail_list.Add(new mailselectList
                                    {
                                        module = "International SOS Record",
                                        mail_to = email_broker,
                                        mail_to_display = email_broker_name,
                                        //mail_cc = email_user_in_doc + email_admin,
                                        mail_cc = email_admin,
                                        mail_body_in_form = "",
                                        mail_status = "true",
                                        action_change = "true",
                                        emp_id = emp_id_user_in_doc,
                                    });

                                    ret = "";
                                    SendEmailService swmail = new SendEmailService();
                                    ret = swmail.SendMailInPage(ref mail_list, data.emp_list, data.img_list, data.doc_id
                                        , page_name, module_name);
                                    if (ret.ToLower() != "true")
                                    {
                                        msg_error = ret;
                                    }
                                    else if (msg_error == "true")
                                    {
                                        //update 
                                        sqlstr = @" UPDATE BZ_DOC_ISOS_RECORD SET SEND_MAIL_TYPE = 1";
                                        sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);
                                        sqlstr += @" ,UPDATE_DATE = sysdate";
                                        sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                                        sqlstr += @" where ";
                                        sqlstr += @" DOC_ID = " + conn.ChkSqlStr(value.doc_id, 300);
                                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, false);
                                    }

                                }
                            }
                            else
                            {
                                ret = "false";
                                msg_error = "ไม่มีรายการที่ต้องส่ง";
                            }

                        }

                    }
                }

            }

            if ((ret.ToLower() ?? "") == "true")
            {
                SearchDocService swd = new SearchDocService();
                ISOSModel value_load = new ISOSModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new ISOSOutModel();
                data = swd.SearchISOS(value_load);
                msg_error += "Load Data";

            }
            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public PortalOutModel SendMailContact(PortalOutModel value)
        {
            //get in touch เมื่อ submit ให้ส่ง mail หา admin & CONTACT US

            var data = value;
            var msg_error = "";
            var role_type = "pmsv_admin";
            var mail_body_in_form = value.text_name + "<br>" + value.text_subject + "<br>" + value.text_message;

            SearchDocService swd = new SearchDocService();
            DataTable dtemplist = swd.refsearch_emprole_list(role_type);
            string email_admin = "";
            for (int i = 0; i < dtemplist.Rows.Count; i++)
            {
                email_admin += dtemplist.Rows[i]["email"] + ";";
            }

            SearchDocService _swd = new SearchDocService();
            dtemplist = _swd.refsearch_emprole_list("pmdv_admin");
            for (int i = 0; i < dtemplist.Rows.Count; i++)
            {
                email_admin += dtemplist.Rows[i]["email"] + ";";
            }

            //ให้ cc หาคนที่แจ้งปัญหาด้วย จับจาก token login 
            var mail_cc_active = "";
            mail_cc_active = sqlEmpUserMail(value.token_login);

            List<mailselectList> mail_list = new List<mailselectList>();
            mail_list.Add(new mailselectList
            {
                module = "Contact As",
                mail_to = value.text_contact_email + ";" + email_admin,
                mail_body_in_form = mail_body_in_form,
                mail_cc = mail_cc_active,
                mail_status = "true",
                action_change = "true",
            });

            ret = "";
            SendEmailService swmail = new SendEmailService();
            ret = swmail.SendMailInContact(ref mail_list);

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }

        #endregion set send mail in page 


        #region SAP
        public TravelExpenseOutModel SendTravelExpenseToSAP(TravelExpenseOutModel value)
        {
            var msg = "";
            var sqlstr_all = "";
            var page_name = "travelexpense";
            var imglist = new List<ImgList>();
            var token_login = value.token_login;
            var doc_id = value.doc_id;

            var data = new TravelExpenseOutModel();
            data = value;
            data.token_login = token_login;
            data.doc_id = doc_id;
            data.id = "1";
            data.user_admin = true;

            //ทดสอบ update status = Send to SAP ทั้งหมดใน list ที่ส่งไป SAP ก่อน 
            for (int i = 0; i < data.travelexpense_detail.Count; i++)
            {
                conn = new cls_connection_ebiz();
                string emp_user_active = "";
                string id = data.travelexpense_detail[i].id;
                string emp_id = data.travelexpense_detail[i].emp_id;
                string status_sap = "";

                List<EmpListOutModel> dremplist = data.emp_list.Where(a => ((a.emp_id == emp_id) && (a.send_to_sap == "true"))).ToList();
                if (dremplist.Count > 0) { status_sap = "6"; } else { continue; }
                if (data.travelexpense_detail[i].status_active == "true") { status_sap = "6"; } else { continue; }

                data.travelexpense_detail[i].status = status_sap;

                sqlstr = @" update BZ_DOC_TRAVELEXPENSE_DETAIL set";

                sqlstr += @" STATUS = " + conn.ChkSqlStr(status_sap, 4000);

                sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                sqlstr += @" ,UPDATE_DATE = sysdate";
                sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                sqlstr += @" where ";
                sqlstr += @" ID = " + conn.ChkSqlStr(id, 300);
                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(doc_id, 300);
                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(emp_id, 300);

                ret = conn_ExecuteNonQuery(sqlstr, true);
                sqlstr_all += sqlstr + "||";

                if (ret.ToLower() != "true") { goto Next_line_1; }
            }
        Next_line_1:;

            if (ret.ToLower() == "true")
            {
                ret = conn_ExecuteNonQuery(sqlstr_all, false); sqlstr_all = "";
            }

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchDocService swd = new SearchDocService();
                TravelExpenseModel value_load = new TravelExpenseModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new TravelExpenseOutModel();
                data = swd.SearchTravelExpense(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send data to SAP succesed." : "Send data to SAP failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;
            return data;
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

        #endregion SAP

        #region car service
        //CarServiceOutModel
        public TransportationOutModel OpenWebCarService(TransportationOutModel value)
        {
            //ส่ง mail  ส่งให้พนักงานที่อยู่ในใบงาน 
            //mail isos : to all user ในใบงานนั้นๆ, cc pmsv group
            var data = value;
            var msg_error = "";
            var page_name = "transportation";
            var module_name = "";
            var doc_id = value.doc_id.ToString();

            #region Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed
            string doc_status = "";
            var emp_id_select = "";
            List<EmpListOutModel> drempcheck = data.emp_list.Where(a => (a.mail_status == "true")).ToList();
            if (drempcheck.Count > 0)
            {
                emp_id_select = drempcheck[0].emp_id;
                if (value.user_admin == true)
                {
                    doc_status = "3";
                }
                else
                {
                    doc_status = "2";
                }
                try
                {
                    if (drempcheck[0].doc_status_id.ToString() == "4") { doc_status = "4"; }
                }
                catch { }

                drempcheck[0].doc_status_id = doc_status;

            }

            sqlstr = @"delete from BZ_DATA_CONTENT_EMP where doc_id = '" + doc_id.ToString() + "' and emp_id = '" + emp_id_select + "' ";
            ret = conn_ExecuteNonQuery(sqlstr, false);

            sqlstr = @"insert into  BZ_DATA_CONTENT_EMP ( doc_id, emp_id, doc_status) values ( '" + doc_id.ToString() + "', '" + emp_id_select + "', '" + doc_status + "' )";
            ret = conn_ExecuteNonQuery(sqlstr, false);
            #endregion Auwat 20210823 0000 เพิ่มข้อมูล status ของใบงาน --> 1: Not Start, 2: Traveler, 3: Business Team, 4: Completed


            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;


            return data;
        }

        #endregion car service

        #region Insert User Contract
        public string AddUser(string user, ref string _msg)
        {
            List<Users> userList = new List<Users>();
            string xuser_ad = user.Replace("@thaioilgroup.com", "");
            string xpass_ad = "admin";
            string employee_id = "";

            Boolean bUserInThaioilGroup = false;

            //กรณีที่มาจาก AD หรือมาจากหน้า Login ที่เป็นการสวมสิทธิ์ 
            userAuthenService userAuthen = new userAuthenService();
            userList = userAuthen.GetADUsersFilter(xuser_ad, xpass_ad, ref _msg);
            try
            {
                if (_msg == "")
                {
                    if (userList[0].UserName.ToString().ToLower() == xuser_ad.ToLower())
                    {
                        bUserInThaioilGroup = true;
                    }
                    else { _msg += "invalid username(1) :" + xuser_ad; }
                }
            }
            catch { _msg += "invalid username(2):" + xuser_ad + " group : " + bUserInThaioilGroup; }

            if (bUserInThaioilGroup == true)
            {
                DataTable dt = new DataTable();
                sqlstr = @" select EMPLOYEEID
                        ,case when USERTYPE = 2 then 'Y' else 'N' end  check_user_z 
                        from VW_BZ_USERS where upper(userid) =  upper('" + user + "')";
                if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                {
                    string action_type = "";
                    string token_login = Guid.NewGuid().ToString();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow login_empid = dt.Rows[0];
                        employee_id = login_empid["EMPLOYEEID"].ToString() ?? "";

                        if (login_empid["EMPLOYEEID"].ToString() == "Y")
                        {
                            //อัฟเดตข้อมูลในตารางกรณีที่เป็น user z
                            action_type = "update";
                        }
                    }
                    else
                    {
                        //กรณีที่ไม่มีข้อมูลจะเป็น user z ทั้งหมด กรณีที่เป็น user thaioilจะต้องมีใน table อยู่แล้ว?? มี batch ดึงข้อมูล
                        action_type = "insert";//new empid 
                    }

                    if (userList != null)
                    {
                        if (userList.Count > 0)
                        {
                            //เพิ่ม/update ข้อมูลในตาราง BZ_USERS  
                            sqlstr = @" call bz_sp_add_user_z ( ";
                            sqlstr += " '" + token_login + "'";
                            sqlstr += ",'" + user.ToString().ToUpper() + "'";
                            sqlstr += ",'" + userList[0].DisplayName.ToString() + "'";
                            sqlstr += ",'" + userList[0].Email.ToString() + "'";
                            sqlstr += ",'" + action_type + "'";
                            sqlstr += ")";
                            if (SetDocService.conn_ExecuteNonQuery(sqlstr, false) == "true")
                            {
                                sqlstr = "select * from  VW_BZ_USERS where upper(USERID) =  '" + user.ToString().ToUpper() + "'";
                                if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        employee_id = dt.Rows[0]["EMPLOYEEID"].ToString() ?? "";
                                        _msg = "";
                                        return employee_id;
                                    }
                                }
                            }

                        }
                    }

                }

            }

            return employee_id;
        }
        #endregion Insert User Contract
    }



}