
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

using Microsoft.EntityFrameworkCore;
using top.ebiz.service.Models.Create_Trip;

namespace top.ebiz.service.Service.Create_Trip
{
    public class userAuthenService
    {
        public List<loginProfileResultModel> getProfile(loginProfileModel value)
        {
            var data = new List<loginProfileResultModel>();

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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

        public List<Users> GetADUsers(string UserName)
        {
            //DevFix 20241017 0000 ดึงข้อมูลจาก Table 
            List<Users> lstADUsers = new List<Users>();

            try
            {
                using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
                {
                    using (var connection = context.Database.GetDbConnection())
                    {
                        connection.Open();

                        //DevFix 20241017 0000 ที่จริงต้องเอามาจาก AzureAD เนื่องจากจะเอาไปตรวจสอบว่า User ที่ Login เข้ามา ถ้าเป็น z user ให้ Add เพิ่ม
                        var dt = context.VW_BZ_USERS
                         .FromSqlRaw("select email, userdisplay  from vw_bz_users h where lower(userid) = lower(:userid) "
                        , context.ConvertTypeParameter("userid", UserName, "char")).ToList();

                        if (dt != null)
                        {
                            if (dt?.Count > 0)
                            {
                                Users objSurveyUsers = new Users();
                                objSurveyUsers.Email = dt[0].EMAIL;
                                objSurveyUsers.UserName = dt[0].USERID;
                                objSurveyUsers.DisplayName = dt[0].USERDISPLAY;

                                lstADUsers.Add(objSurveyUsers);
                            }
                        }

                    }
                }
            }
            catch { }
            return lstADUsers;
        }

        public loginWebResultModel loginWeb(loginClientModel value)
        {
            value.user = (value.user ?? "") == "" ? "" : value.user ?? "".ToUpper();

            string xuser_ad = value.user; //email

            string token_login = Guid.NewGuid().ToString() ?? "";
            var data = new loginWebResultModel();
            var userList = GetADUsers(xuser_ad);
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


            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    //string sql = "select EMPLOYEEID from BZ_USERS where upper(userid) = '" + value.user.ToUpper() + "'";
                    //var user = context.Database.SqlQuery<loginUserResultModel>(sql).ToList();
                    var userid = value.user ?? "";
                    var userdisplay = userList[0].DisplayName.ToString() ?? "";
                    var user = context.BZ_USERS
                         .FromSqlRaw("select upper(userid) as userid, employeeid from BZ_USERS where upper(userid) = upper(:userid) "
                         , context.ConvertTypeParameter("userid", userid, "char")).ToList();

                    if (user != null && user.Count() > 0)
                    {
                        DbCommand cmd = connection.CreateCommand();
                        cmd.CommandText = "bz_sp_login_token2";
                        cmd.CommandType = CommandType.StoredProcedure;
                        //string token_login = Guid.NewGuid().ToString();//DevFix 20201209 1100ยกไปส่วนบน
                        cmd.Parameters.Add(new OracleParameter("p_token", token_login));
                        cmd.Parameters.Add(new OracleParameter("p_user_id", user[0].EMPLOYEEID));
                        cmd.Parameters.Add(new OracleParameter("p_user_name", user[0].USERID));
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
                        //sql = "select EMPLOYEEID from BZ_USERS_AD where upper(userid) = '" + value.user.ToUpper() + "'";
                        //user = new List<loginUserResultModel>();
                        //user = context.Database.SqlQuery<loginUserResultModel>(sql).ToList();

                        user = context.BZ_USERS
                   .FromSqlRaw("select upper(userid) as userid, employeeid from BZ_USERS_AD where upper(userid) = upper(:userid) "
                   , context.ConvertTypeParameter("userid", userid, "char")).ToList();

                        if (user != null && user.Count() > 0)
                        {
                            bUserAD = true;
                        }
                        else
                        {
                            //Get Max EMPLOYEEID (autogenarate) --> bz_sp_add_user_ad   
                            DbCommand cmd = connection.CreateCommand();
                            cmd.CommandText = "bz_sp_add_user_ad";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new OracleParameter("p_token", token_login));
                            cmd.Parameters.Add(new OracleParameter("p_user_id", userid));
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
                            //sql = "select EMPLOYEEID from BZ_USERS_AD where upper(userid) = '" + value.user.ToUpper() + "'";
                            //user = new List<loginUserResultModel>();
                            //user = context.Database.SqlQuery<loginUserResultModel>(sql).ToList();
                            user = context.BZ_USERS
                   .FromSqlRaw("select upper(userid) as userid, employeeid from BZ_USERS_AD where upper(userid) = upper(:userid) "
                   , context.ConvertTypeParameter("userid", userid, "char")).ToList();
                            if (user != null && user.Count() > 0)
                            {
                                DbCommand cmd = connection.CreateCommand();
                                cmd.CommandText = "bz_sp_login_token2";
                                cmd.CommandType = CommandType.StoredProcedure;
                                //string token_login = Guid.NewGuid().ToString();//DevFix 20201209 1100ยกไปส่วนบน
                                cmd.Parameters.Add(new OracleParameter("p_token", token_login));
                                cmd.Parameters.Add(new OracleParameter("p_user_id", user[0].EMPLOYEEID));
                                cmd.Parameters.Add(new OracleParameter("p_user_name", userdisplay));
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

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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