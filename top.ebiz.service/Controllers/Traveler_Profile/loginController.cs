//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Net;
//using System.Net.Http;
////using System.Web.Http;
////using System.Web.Script.Serialization;

//using System.Text.Json;
//using top.ebiz.service.Models.Traveler_Profile;
//using top.ebiz.service.Service.Traveler_Profile;
//using Microsoft.AspNetCore.Mvc;
//using Oracle.ManagedDataAccess.Client;

//namespace top.ebiz.service.Controllers.Traveler_Profile
//{
//   // [ApiController]
//    //[Route("api/[controller]")]
//    public class loginController : ControllerBase
//    {
//        // GET: api/loginWeb
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET: api/loginWeb/5
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST: api/loginWeb
//       [IgnoreAntiforgeryToken]
//        [HttpPost("login", Name = "login")]
//public IActionResult Post([FromBody]loginModel value)
//        {
//            if (value == null) return null;

            
//            logService.logModel mLog = new logService.logModel();
//            mLog.module = "login";
//            mLog.tevent = "";
//            mLog.ref_id = 0;
//            mLog.data_log = JsonSerializer.Serialize(value);
//            logService.insertLog(mLog);


//            HttpResponseMessage response = null;
//            userAuthenService service = new userAuthenService();
//            Object result = service.login(value);

//            #region test 
//            if (false)
//            {
//                int iResult = -1;
//                var ret = "";
//                var sqlstr = @" call bz_sp_insert_log(";
//                sqlstr += @" 'Test " + mLog.module + "'";
//                sqlstr += @" ,'" + mLog.tevent + "'";
//                sqlstr += @" ,'" + mLog.data_log + "'";
//                sqlstr += @" ,'" + mLog.ref_id + "'";
//                sqlstr += @" ,'" + mLog.ref_code + "'";
//                sqlstr += @" ,'" + mLog.user_log + "'";
//                sqlstr += @" ,'" + mLog.user_token + "'";
//                sqlstr += @" )";

//                ret = SetDocService.conn_ExecuteNonQuery(sqlstr, false);

//                result = "ret:" + ret + "-- >sqlstr:" + sqlstr;


//                sqlstr = @" SELECT a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ,a.TOKEN_CODE as token_code
//                        FROM bz_login_token a inner join vw_bz_users u on a.user_login = u.userid
//                        WHERE a.TOKEN_CODE = '" + value.token_login + "'  ";

//                ret = "";
//                DataTable dtrole = new DataTable();
//                //cls_connection_ebiz conn = new cls_connection_ebiz();
//                //conn.OpenConnection(); 
//                if (SetDocService.conn_ExecuteData(ref dtrole, sqlstr) == "")
//                {
//                    if (dtrole.Rows.Count > 0)
//                    {
//                        ret = dtrole.Rows[0]["user_name"].ToString();

//                    }
//                    else
//                    {
//                        ret = "ไม่ข้อมูล User ในระบบ";
//                    }
//                }
//                result += "-->select case ret:" + ret + "-- >sqlstr:" + sqlstr; 
//            }
//            #endregion test

//            string json = JsonSerializer.Serialize(result);

//            return Ok(json);
//        }

//        // PUT: api/loginWeb/5
//        public void Put(int id, [FromBody]string value)
//        {
//        }

//        // DELETE: api/loginWeb/5
//        public void Delete(int id)
//        {
//        }
//    }
//}
