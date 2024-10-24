using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.DirectoryServices;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;
using top.ebiz.service.Models.Create_Trip;

namespace top.ebiz.service.Service.Create_trip
{
    public class userAuthenService
    {
        public List<loginProfileResultModel> getProfile(loginProfileModel value)
        {
            var data = new List<loginProfileResultModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_login_profile";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_token", value.token_login));


                    OracleParameter oraP = new OracleParameter();
                    oraP.ParameterName = "mycursor";
                    oraP.OracleDbType = OracleDbType.RefCursor;
                    oraP.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP);

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            var schema = reader.GetSchemaTable();
                            data = reader.MapToList<loginProfileResultModel>() ?? new List<loginProfileResultModel>();
                        }
                        catch (Exception ex) { }
                    }

                }
            }

            return data;
        }

        public loginResultModel login(loginModel value)
        {
            var data = new loginResultModel();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_login_token";
                    cmd.CommandType = CommandType.StoredProcedure;

                    string token_login = Guid.NewGuid().ToString();
                    cmd.Parameters.Add(new OracleParameter("p_token", token_login));
                    cmd.Parameters.Add(new OracleParameter("p_user_id", value.user_id));

                    //OracleParameter oraP = new OracleParameter();
                    //oraP.ParameterName = "token_login";
                    ////oraP.OracleDbType = OracleDbType.RefCursor;
                    ////oraP.Direction = ParameterDirection.Output;
                    //
                    //cmd.Parameters.Add(token_login);

                    try
                    {
                        cmd.ExecuteNonQuery();


                        data.msg_sts = "S";
                        data.msg_txt = "success";
                        data.token_login = token_login;
                    }
                    catch (Exception ex)
                    {
                        data.msg_sts = "E";
                        data.msg_txt = "Error";
                        data.token_login = ex.Message.ToString();
                    }


                }
            }

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
                string domain = "";// ddlCompany.SelectedItem.Text;//   '"IPT"
                domain = "ThaioilNT";
                string str = string.Format("LDAP://{0}", domain);
                string str2 = string.Format("{0}\\" + UserName.Trim(), domain);
                string pass = Password;
                DirectoryEntry Entry = new DirectoryEntry(str, str2, pass);
                DirectorySearcher search = new DirectorySearcher(Entry);
                //SearchResultCollection results;
                search.Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=*))";
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("usergroup");
                search.PropertiesToLoad.Add("displayname");//first name
                                                           //search.PropertiesToLoad.Add("memberOf");
                SearchResult result;
                SearchResultCollection resultCol = search.FindAll();


                //SearchResult xxx = search.FindOne();
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        string UserNameEmailString = string.Empty;
                        result = resultCol[counter];
                        if (result.Properties.Contains("samaccountname") &&
                                 result.Properties.Contains("mail") &&
                            result.Properties.Contains("displayname"))
                        {

                            Users objSurveyUsers = new Users();
                            objSurveyUsers.Email = (string)result.Properties["mail"][0] +
                              "^" + (string)result.Properties["displayname"][0];
                            objSurveyUsers.UserName = (string)result.Properties["samaccountname"][0];
                            objSurveyUsers.DisplayName = (string)result.Properties["displayname"][0];
                            //objSurveyUsers.MemberOf = (String)result.Properties["memberOf"][0];
                            lstADUsers.Add(objSurveyUsers);

                        }

                    }
                }


                return lstADUsers;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public loginWebResultModel loginWeb(loginClientModel value)
        {
            value.user = (value.user ?? "") == "" ? "" : value.user.ToUpper();

            string xuser_ad = value.user;
            string xpass_ad = value.pass;
            if (value.pass == "admin")
            {
                xuser_ad = "zad";
            }

            string token_login = Guid.NewGuid().ToString();
            var data = new loginWebResultModel();
            var userList = GetADUsers(xuser_ad, xpass_ad);
            if (userList == null)
            {
                if (value.pass == "admin")
                {
                    data.name = value.user ?? "";
                    data.token = token_login ?? "";
                }
                else
                {
                    data.token = "";
                    data.name = "invalid username or password! (ad)";
                    return data;
                }
            }

            if (value.pass == "admin")
            {
                data.name = value.user ?? "";
                data.token = token_login ?? "";
            }
            #region DevFix 20201209 1100 เพิ่มเงื่อนไขกรณีที่เป็น pass : admin ให้สามารถเข้าระบบได้เลย เนื่องจากอาจจะเป็นการทดสอบระบบ ของ admin

            if (userList == null)
            {
                if (value.pass == "admin")
                {
                    data.name = value.user ?? "";
                    data.token = token_login ?? "";
                }
                else
                {
                    data.token = "";
                    data.name = "invalid username or password! (ad)";
                    return data;
                }
            }
            if (value.pass == "admin")
            {
                data.name = value.user ?? "";
                data.token = token_login ?? "";
            }
            #endregion DevFix 20201209 1100 เพิ่มเงื่อนไขกรณีที่เป็น pass : admin ให้สามารถเข้าระบบได้เลย เนื่องจากอาจจะเป็นการทดสอบระบบ ของ admin



            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    string sql = "select EMPLOYEEID from BZ_USERS where upper(userid) = '" + value.user.ToUpper() + "'";

                    var user = context.Database.SqlQuery<loginUserResultModel>(sql).ToList();
                    if (user != null && user.Count() > 0)
                    {
                        DbCommand cmd = connection.CreateCommand();
                        cmd.CommandText = "bz_sp_login_token2";
                        cmd.CommandType = CommandType.StoredProcedure;
                        //string token_login = Guid.NewGuid().ToString();//DevFix 20201209 1100ยกไปส่วนบน
                        cmd.Parameters.Add(new OracleParameter("p_token", token_login));
                        cmd.Parameters.Add(new OracleParameter("p_user_id", user[0].EMPLOYEEID));
                        cmd.Parameters.Add(new OracleParameter("p_user_name", value.user.ToUpper()));
                        try
                        {
                            cmd.ExecuteNonQuery();

                            data.token = token_login;
                        }
                        catch (Exception ex)
                        {
                            data.token = "E";
                            data.name = ex.Message.ToString();
                        }
                    }
                    else
                    {
                        //data.token = "";
                        //data.name = "invalid username or password!";
                        #region DevFix 20201209 1100 กรณีที่เป็น user z ที่ผ่านการ login ad แล้ว
                        bool bUserAD = false;
                        sql = "select EMPLOYEEID from BZ_USERS_AD where upper(userid) = '" + value.user.ToUpper() + "'";
                        user = new List<loginUserResultModel>();
                        user = context.Database.SqlQuery<loginUserResultModel>(sql).ToList();
                        if (user != null && user.Count() > 0)
                        {
                            bUserAD = true;
                        }
                        else
                        {
                            //Get Max EMPLOYEEID (autogenarate) --> bz_sp_add_user_ad  
                            //create or replace procedure bz_sp_add_user_ad(p_token varchar2, p_user_id varchar2, p_user_name varchar2) IS 
                            //  BEGIN 
                            //  insert into BZ_TRANS_LOG(MODULE, USER_TOKEN, LASTUPDATED_LOG) values('ADD USER AD', p_user_name, sysdate); 
                            //  insert into BZ_USERS_AD(EMPLOYEEID, USERID, ENFIRSTNAME, UPDATEDDATE)
                            //  values((select nvl(max(to_number(EMPLOYEEID)), 90000000) + 1 from BZ_USERS_AD ) , p_user_id, p_user_name, sysdate); 
                            //  END bz_sp_add_user_ad;

                            DbCommand cmd = connection.CreateCommand();
                            cmd.CommandText = "bz_sp_add_user_ad";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new OracleParameter("p_token", token_login));
                            cmd.Parameters.Add(new OracleParameter("p_user_id", value.user.ToUpper()));
                            cmd.Parameters.Add(new OracleParameter("p_user_name", userList[0].DisplayName.ToString()));
                            try
                            {
                                cmd.ExecuteNonQuery();

                                bUserAD = true;
                            }
                            catch (Exception ex)
                            {
                                bUserAD = false;
                            }
                        }


                        if (bUserAD == true)
                        {
                            sql = "select EMPLOYEEID from BZ_USERS_AD where upper(userid) = '" + value.user.ToUpper() + "'";
                            user = new List<loginUserResultModel>();
                            user = context.Database.SqlQuery<loginUserResultModel>(sql).ToList();

                            if (user != null && user.Count() > 0)
                            {
                                DbCommand cmd = connection.CreateCommand();
                                cmd.CommandText = "bz_sp_login_token2";
                                cmd.CommandType = CommandType.StoredProcedure;
                                //string token_login = Guid.NewGuid().ToString();//DevFix 20201209 1100ยกไปส่วนบน
                                cmd.Parameters.Add(new OracleParameter("p_token", token_login));
                                cmd.Parameters.Add(new OracleParameter("p_user_id", user[0].EMPLOYEEID));
                                cmd.Parameters.Add(new OracleParameter("p_user_name", value.user.ToUpper()));
                                try
                                {
                                    cmd.ExecuteNonQuery();

                                    data.token = token_login;
                                }
                                catch (Exception ex)
                                {
                                    data.token = "E";
                                    data.name = ex.Message.ToString();
                                }
                            }
                        }
                        else
                        {
                            data.token = "";
                            data.name = "invalid username or password!";
                        }
                        #endregion DevFix 20201209 1100 กรณีที่เป็น user z ที่ผ่านการ login ad แล้ว
                    }


                }
            }

            return data;
        }

        public loginResultModel logout(logoutModel value)
        {
            var data = new loginResultModel();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_logout";
                    cmd.CommandType = CommandType.StoredProcedure;

                    //string token_login = Guid.NewGuid().ToString();
                    cmd.Parameters.Add(new OracleParameter("p_token", value.token));

                    try
                    {
                        cmd.ExecuteNonQuery();

                        data.msg_sts = "S";
                        data.msg_txt = "success";
                        data.token_login = "";
                    }
                    catch (Exception ex)
                    {
                        data.msg_sts = "E";
                        data.msg_txt = "Error";
                        data.token_login = "";
                    }


                }
            }

            return data;
        }

    }
}