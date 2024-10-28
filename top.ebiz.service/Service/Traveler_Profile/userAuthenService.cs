using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.DirectoryServices;
using top.ebiz.service.Models.Traveler_Profile;

using System.Text.Json;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;
//using System.Data.OracleClient;
//using static ebiz.webservice.Service.logService;
//using System.Web.Script.Serialization;

namespace top.ebiz.service.Service.Traveler_Profile 
{

    public class userAuthenService
    {
        string sqlstr = "";
        string ret = "";
        string SystemUser = System.Configuration.ConfigurationManager.AppSettings["SystemUser"].ToString();
        string SystemPass = System.Configuration.ConfigurationManager.AppSettings["SystemPass"].ToString();

        private void call_SetGroupSystemAdmin()
        {
            //เพิ่ม Set Emp in Group Admin??? อาจจะต้องไป set batch
            SetDocService swd = new SetDocService();
            swd.SetGroupSystemAdmin();
        }
        public List<loginProfileResultModel> getProfile(loginProfileModel value)
        {
            var data = new List<loginProfileResultModel>();
            var msg = "";
            Boolean bCheckDataAD = false;
            ret = ""; sqlstr = "";
            DataTable dt = new DataTable();
            sqlstr = @"  select  distinct   u.EMPLOYEEID empId
                    , nvl(ENTITLE,'')|| ' ' || ENFIRSTNAME||' '||ENLASTNAME empName
                    , u.COMPANYCODE deptName 
                      , case when to_number(u.employeeid) < 1000000   then 
                                (case when up.imgpath is null then replace(replace(replace(u.imgurl,'.jpg', ''),u.employeeid, ''),to_number(u.employeeid),'') else up.imgpath end)
                              else 
                                (case when up.imgpath is null then replace((replace(replace(u.imgurl,'.jpg', ''),u.employeeid, '')),'/TOP/','/TES/') else up.imgpath end)
                      end  
                      ||
                      case when to_number(u.employeeid) < 1000000   then 
                                (case when up.imgprofilename is null then to_char(to_number(u.employeeid)) || '.jpg' else up.imgprofilename end)
                                else 
                                (case when up.imgprofilename is null then to_char((u.employeeid)) || '.jpg' else up.imgprofilename end)
                     end imgUrl
                     , u.usertype as user_type
                     from      BZ_LOGIN_TOKEN t 
                     left join vw_bz_users u on t.user_login = u.USERID
                     left join bz_user_peofile up on u.employeeid = up.employeeid
                     where     t.TOKEN_CODE = '" + value.token_login + "'";
            if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
            {
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["empid"].ToString() != "")
                    {
                        bCheckDataAD = false;
                    }
                }
            }

            Boolean user_admin = false;
            string token_login = value.token_login;
            //ea1d088c-3e0a-433c-acca-f6a16beab66a
            sqlstr = @" SELECT a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code
                        FROM bz_login_token a left join vw_bz_users u on a.user_login = u.userid
                        WHERE a.TOKEN_CODE = '" + token_login + "'  ";

            DataTable dtrole = new DataTable();
            if (SetDocService.conn_ExecuteData(ref dtrole, sqlstr) == "")
            {
                if (dtrole.Rows.Count == 0)
                {
                    
                    logService.logModel mLog = new logService.logModel();
                    mLog.module = "Login";
                    mLog.tevent = "login";
                    mLog.data_log = JsonSerializer.Serialize(value);
                    mLog.user_token = token_login;
                    logService.insertLog(mLog);
                    if (SetDocService.conn_ExecuteData(ref dtrole, sqlstr) == "")
                    {
                    }
                }
                if (dtrole.Rows.Count > 0)
                {
                    DataRow login_empid = dtrole.Rows[0];
                    if ((login_empid["user_role"].ToString() ?? "") == "1") { user_admin = true; }
                }
            }


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data.Add(new loginProfileResultModel
                {
                    empId = dt.Rows[i]["empId"].ToString(),
                    empName = dt.Rows[i]["empName"].ToString(),
                    deptName = dt.Rows[i]["deptName"].ToString(),
                    imgUrl = dt.Rows[i]["imgUrl"].ToString(),
                    remark = bCheckDataAD.ToString(),
                    user_admin = user_admin,
                    token_login = token_login,

                    user_type = dt.Rows[i]["user_type"].ToString(),

                }); ;
            }
            return data;
        }

        public loginResultModel login(loginModel value)
        {
            //ปิดเนื่องจากมีการ maintain ผ่านระบบ
            //call_SetGroupSystemAdmin();
            var data = new loginResultModel();
            try
            {
                string token_login = Guid.NewGuid().ToString();
                token_login = value.token_login;
                string user_id = value.user_id + "";
                string user_name = value.user_name + "";

                Boolean bCheckUserInSystem = false;
                if (user_id == "" || user_name == "")
                {
                    sqlstr = @" SELECT a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code
                        FROM bz_login_token a inner join vw_bz_users u on a.user_login = u.userid
                        WHERE a.TOKEN_CODE = '" + token_login + "'  ";

                    DataTable dtrole = new DataTable();
                    if (SetDocService.conn_ExecuteData(ref dtrole, sqlstr) == "")
                    {
                        if (dtrole.Rows.Count > 0)
                        {
                            user_id = dtrole.Rows[0]["user_id"].ToString();
                            user_name = dtrole.Rows[0]["user_name"].ToString();
                            bCheckUserInSystem = true;
                        }
                        else 
                        {
                            ret = "ไม่ข้อมูล User ในระบบ";
                        }
                    }

                }

                int iret = -1;
                if (bCheckUserInSystem == true)
                {

                    logService.logModel mLog = new logService.logModel();
                    mLog.module = "Login";
                    mLog.tevent = "login after check user in table";
                    mLog.data_log = JsonSerializer.Serialize(value);
                    mLog.user_token = token_login;
                    logService.insertLog(mLog);

                    
                    loginModel mLogin = new loginModel();
                    mLogin.token_login = token_login;
                    mLogin.user_id = user_id;
                    mLogin.user_name = user_name;
                    iret = logService.insertLogin(mLogin);

                    //Auwat 20210831 0000 กรณีที่ login ไม่ได้ คิดว่าน่าจะติดเรื่อง table ข้อมูลเต็ม
                    if (iret == -1)
                    {
                        //delete ข้อมูลใน table 
                    }

                }
                if (iret == 1)
                {
                    data.msg_sts = "S";
                    data.msg_txt = "success";
                    data.token_login = token_login;
                }
                else
                {
                    data.msg_sts = "E";
                    data.msg_txt = "Error1";
                    data.token_login = ret.ToString() + sqlstr;
                }
            }
            catch (Exception ex)
            {
                data.msg_sts = "E";
                data.msg_txt = "Error2";
                data.token_login = ex.Message.ToString() + sqlstr;
            }

            return data;
        }

        public loginWebResultModel loginWeb(loginClientModel value)
        {
            //ปิดเนื่องจากมีการ maintain ผ่านระบบ
            //call_SetGroupSystemAdmin();

            value.user = (value.user ?? "") == "" ? "" : value.user.ToUpper();
            //auwat 20210215 0901
            //เดียวต้องเพิ่มให้ดึงข้อมูลของ z user ใหม่ตลอก เพื่อกรณีที่เป็นการ update display name
            //เก็บข้อมูล e-mail ด้วย
            //ต้องทำ config user กลาง ?? เพื่อเข้า ad กรณีที่จะสวมสิทธิ์ z user

            string token_login = Guid.NewGuid().ToString();
            var data = new loginWebResultModel();
            List<Users> userList = new List<Users>();
            string xuser_ad = value.user;
            string xpass_ad = value.pass;
            string xuser_id = "";
            string _msg_ad = "";
             
            if (value.pass == "admin")
            {
                //xuser_ad = "zad";
                //กรณีที่มาจาก AD หรือมาจากหน้า Login ที่เป็นการสวมสิทธิ์ 
                userList = GetADUsersFilter(xuser_ad, xpass_ad, ref _msg_ad);
            }
            else
            {
                //มาจากหน้า Login ที่เป็นการกรอกข้อมูล user/pass เอง
                userList = GetADUsers(xuser_ad, xpass_ad);
            }


            Boolean bUserInThaioilGroup = false;
            if (userList == null)
            {
                if (value.pass == "admin")
                {
                    data.name = value.user ?? "";
                    data.token = token_login ?? ""; 
                }
                else
                {
                    data.msg_sts = "false";
                    data.token = "";
                    data.name = "invalid username or password! (ad)";
                    data.msg_txt = "invalid username or password!";
                    return data;
                }
            }
            try
            {
                if (userList[0].UserName.ToString().ToLower() == xuser_ad.ToLower())
                {
                    bUserInThaioilGroup = true;
                }
            }
            catch { }

            if (bUserInThaioilGroup == false)
            {
                data.msg_sts = "false";
                data.token = "";
                data.name = "invalid username or password! (ad)";
                data.msg_txt = "invalid username or password!";
                return data;
            }
 
            if (value.pass == "admin")
            {
                data.name = value.user ?? "";
                data.token = token_login ?? "";
            }

             
            Boolean bLoginError = false;
            DataTable dt = new DataTable();
            sqlstr = @" select EMPLOYEEID
                        ,case when USERTYPE = 2 then 'Y' else 'N' end  check_user_z 
                        from VW_BZ_USERS where upper(userid) =  upper('" + value.user + "')";
            if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
            {
                string action_type = "";
                if (dt.Rows.Count > 0)
                {
                    DataRow login_empid = dt.Rows[0];
                    data.name = login_empid["EMPLOYEEID"].ToString() ?? "";
                    data.token = token_login ?? "";

                    xuser_id = login_empid["EMPLOYEEID"].ToString() ?? "";

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
                    data.name = value.user ?? "";
                    data.token = token_login ?? "";
                }

                if (userList != null)
                {
                    if (userList.Count > 0)
                    {
                        //เพิ่ม/update ข้อมูลในตาราง BZ_USERS  
                        sqlstr = @" call bz_sp_add_user_z ( ";
                        sqlstr += " '" + token_login + "'";
                        sqlstr += ",'" + value.user.ToString().ToUpper() + "'";
                        sqlstr += ",'" + userList[0].DisplayName.ToString() + "'";
                        sqlstr += ",'" + userList[0].Email.ToString() + "'";
                        sqlstr += ",'" + action_type + "'";
                        sqlstr += ")";
                        if (SetDocService.conn_ExecuteNonQuery(sqlstr, false) == "true")
                        {
                            sqlstr = "select * from  VW_BZ_USERS where upper(USERID) =  '" + value.user.ToString().ToUpper() + "'";
                            if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    xuser_id = dt.Rows[0]["EMPLOYEEID"].ToString() ?? "";
                                }
                            }
                        }

                    }
                }
                else
                {
                    logService.logModel mLogerror = new logService.logModel();
                    mLogerror.module = "Login";
                    mLogerror.tevent = "loginWeb";
                    mLogerror.data_log = "user list is null " + _msg_ad;// JsonSerializer.Serialize(value);
                    mLogerror.user_token = token_login;
                    logService.insertLog(mLogerror);

                    bLoginError = true;
                }


            }



            logService.logModel mLog = new logService.logModel();
            mLog.module = "Login";
            mLog.tevent = "loginWeb";
            mLog.data_log = "user id ref:" + xuser_id;// JsonSerializer.Serialize(value);
            mLog.user_token = token_login;
            logService.insertLog(mLog);

           
            loginModel mLogin = new loginModel();
            mLogin.token_login = token_login;
            mLogin.user_id = xuser_id;
            mLogin.user_name = value.user;
            int iret = logService.insertLogin(mLogin);
            if (iret == 1)
            {
                data.msg_sts = "true";
            }
            if (bLoginError == true & xuser_id == "") {
                data.msg_sts = "false";
                data.msg_txt = "invalid username or password!";
            }

            return data;
        }
        public loginResultModel logout(logoutModel value)
        {

            logService.logModel mLog = new logService.logModel();
            mLog.module = "LogOut";
            mLog.tevent = "logout";
            mLog.data_log = JsonSerializer.Serialize(value);
            mLog.user_token = value.token;
            logService.insertLogOut(mLog);

            var data = new loginResultModel();
            data.msg_sts = "S";
            data.msg_txt = "success";
            data.token_login = "";
            return data;
        }

        public List<Users> GetADUsers(string UserName, string Password)
        {
            try
            {
                List<Users> lstADUsers = new List<Users>();
                //string DomainPath = "LDAP://DC=xxxx,DC=com";
                //DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                //DirectorySearcher search = new DirectorySearcher(searchRoot);
                String domain = "";// ddlCompany.SelectedItem.Text;//   '"IPT"
                domain = "ThaioilNT";
                String str = String.Format("LDAP://{0}", domain);
                String str2 = String.Format(("{0}\\" + UserName.Trim()), domain);
                String pass = Password;
            
                //DirectoryEntry Entry = new DirectoryEntry(str, str2, pass);
                //DirectorySearcher search = new DirectorySearcher(Entry);
                ////SearchResultCollection results;
                ////search.Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=*))";
                //search.Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=*)(samaccountname=" + UserName + "))";
                //search.PropertiesToLoad.Add("samaccountname");
                //search.PropertiesToLoad.Add("mail");
                //search.PropertiesToLoad.Add("usergroup");
                //search.PropertiesToLoad.Add("displayname");//first name

                //                                           //search.PropertiesToLoad.Add("memberOf"); 
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
                //            //objSurveyUsers.MemberOf = (String)result.Properties["memberOf"][0];
                //            lstADUsers.Add(objSurveyUsers);

                //        }

                //    }
                //}


                return lstADUsers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<Users> GetADUsersFilter(string UserName, string Password, ref string _msg_ad)
        {
            try
            {
                string UserName_Def = UserName;
                UserName = SystemUser.ToUpper();
                Password = SystemPass;

                List<Users> lstADUsers = new List<Users>();
                //string DomainPath = "LDAP://DC=xxxx,DC=com";
                //DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                //DirectorySearcher search = new DirectorySearcher(searchRoot);
                String domain = "";// ddlCompany.SelectedItem.Text;//   '"IPT"
                domain = "ThaioilNT";
                String str = String.Format("LDAP://{0}", domain);
                String str2 = String.Format(("{0}\\" + UserName.Trim()), domain);
                String pass = Password;
                //DirectoryEntry Entry = new DirectoryEntry(str, str2, pass);
                //DirectorySearcher search = new DirectorySearcher(Entry);
                ////SearchResultCollection results;
                ////search.Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=*))";
                //search.Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=*)(samaccountname=" + UserName_Def + "))";
                //search.PropertiesToLoad.Add("samaccountname");
                //search.PropertiesToLoad.Add("mail");
                //search.PropertiesToLoad.Add("usergroup");
                //search.PropertiesToLoad.Add("displayname");//first name
                //                                           //search.PropertiesToLoad.Add("memberOf");
                //SearchResult result;
                //SearchResultCollection resultCol = search.FindAll();

                ////SearchResult xxx = search.FindOne();
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
                //            //objSurveyUsers.MemberOf = (String)result.Properties["memberOf"][0];
                //            lstADUsers.Add(objSurveyUsers);

                //        }

                //    }
                //}

                _msg_ad = "";
                return lstADUsers;
            }
            catch (Exception ex)
            {
                _msg_ad = ex.Message.ToString();
                return null;
            }
        }
        public loginAutoResultModel loginauto(loginAutoModel value)
        {
            call_SetGroupSystemAdmin();

            var data = new loginAutoResultModel();
            try
            {

                string token_login = value.token_login;
                string user_id = value.user_id + "";
                string user_name = value.user_name + "";
                string doc_id = value.doc_id + "";

                data.msg_sts = "S";
                data.msg_txt = "";
                data.token_login = token_login;

                //genarate url http://tbkc-dapps-05.thaioil.localnet/Ebiz2/login/6025d3c9-5b7d-4ff9-ac38-585ce35156b4/docid=OB20120008 
                if (doc_id == "")
                {
                    string url = "";
                    //server name --> ServerPath_Web -->http://TBKC-DAPPS-05.thaioil.localnet/ebiz2
                    string _Server_path = System.Configuration.ConfigurationManager.AppSettings["ServerPath_Web"].ToString();
                    url = _Server_path + @"/request/edit/" + doc_id + "/i";
                    //http://tbkc-dapps-05.thaioil.localnet/Ebiz2/main/request/edit/OB20120008/i
                    data.url_next = url;
                }

            }
            catch (Exception ex)
            {
                data.msg_sts = "E";
                data.msg_txt = "Error2";
                data.token_login = ex.Message.ToString();
            }

            return data;
        }

    }


}